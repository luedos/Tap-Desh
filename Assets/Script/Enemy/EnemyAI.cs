using System.Collections;
using System.Collections.Generic;
using UnityEngine;


enum AI_State
{
    Idle = 0,
    Search = 1,
    Attack = 2
}

enum SearchState
{
    ToLastLoc = 0,
    ToNewLoc = 1,
    Waiting = 2
}

public class EnemyAI : MonoBehaviour {
    
    // For unity stupidity
    private bool isLeft = false;

    // For AI
    private AI_State MyState = AI_State.Idle;
    private GameObject enemy = null;
    private bool EnemyTriggered = false;        // For perception (if enemy enter Perception, this start to be true, dosn't meen enemy see player)
    private float TPTimer = 0f;
    private float FireTimer = 0f;
    private Vector3 Location;                   // Generely meen next goal location for AI
    private float ViewRadius = 20f;             // radius for perception (doubled on attack and search state) 
    // For search method
    private Vector3 MyLastLoc;                  // Needed for SearchSpotGenerator
    private SearchState MySearchState = 0;
    private float SearchWaitTimer;              // Timer for waiting on last search state
    private float IncideTPTimer = 0f;           // Because of TP not immediate and takes some time
    private Vector3 EnemyAimPoint;           // Enemy will fire at that point
    [HideInInspector]
    public Vector3 IdleCenter;

    // For Enemy customisation    
    public int[] InvisibleLayers;               // Layers through which AI can see (bullets for example)
    public GameObject Bullet = null;
    public float FireRate = 1.5f;
    public int NumOfBullet = 1;                 // How many bullets will spawned at once
    public float DeltaAngleOfFire = 20f;        // Which angle will be between each bullet (if there more then one)
    public float TPDistance = 8f;
    public float TPRate = 1.5f;
    public CircleCollider2D Perception;
    public U_EQS_Generator FireSpotGenerator = null;        // For finding fire spots
    public U_EQS_Generator SearchSpotGenerator = null;      // For finding spot for last search state
    public U_EQS_Generator IdleSpotGenerator = null;       // For finding next idle spot
    public float BulletLoadLevel = 1f;          // Basicly damage of bullet
    public float SearchWaitingTime = 3f;        // How much time AI will wait in last search state
    public float TPTime = 0.5f;                 // Length of "InsideTPTimer"
    public GameObject TP_Particle = null;
    public GameObject PointerPF = null;         // For player HUD
    [Tooltip("Show how fast enemy can move his aim")]
    public float AimInterpSpeed = 5f;


    public Vector3 StartSerchLocation { get { return MyLastLoc; } }
    public Vector3 Destination { get { return Location; } }
    public GameObject Enemy { get { return enemy; } }
	// Use this for initialization
	void Start () {
        // First of all checking is everything allright
        

        if (Perception != null)
            ViewRadius = Perception.radius;
        else
            print("Out of perception : " + gameObject.name);

        if (Bullet == null)
            print("Out of bullet : " + gameObject.name);

        if (FireSpotGenerator == null)
            print("Out of FireSpotGenerator : " + gameObject.name);

        if (SearchSpotGenerator == null)
            print("Out of SearchSpotGenerator : " + gameObject.name);

        if (IdleSpotGenerator == null)
            print("Out of IdleSpotGenerator : " + gameObject.name);

        if (PointerPF == null)
            print("Out of PointerPF : " + gameObject.name);
        else
        {
            /* As I said this is for player HUD
             * It will attach to player HUD pointer which will point to this enemy */

            // Is HUD object exist in the scene
            GameObject MyHUDObject = GameObject.Find("HUD");
            if (MyHUDObject != null)
            {
                // Because we check only by name we need to know does it has HUDScript
                HUDScript MyHUDScript = MyHUDObject.GetComponent<HUDScript>();
                if (MyHUDScript != null)
                {
                    // If all yes, we create, attach and fill variables in our HUD Pointer
                    GameObject MyGO = Instantiate(PointerPF, MyHUDScript.PointerCenter.transform);
                    MyGO.GetComponent<HUDPointerScript>().Enemy = gameObject;
                    MyGO.GetComponent<HUDPointerScript>().Char = GameObject.FindGameObjectWithTag("Player").transform.Find("Main Camera").gameObject;
                }
            }
            
        }

        GoIdle();

        // Refrech All timers
        FireTimer = FireRate;
        TPTimer = TPRate;
        SearchWaitTimer = SearchWaitingTime;
    }
	
	void Update () {

        
        
        // For make collision allways work ("fuck you" - unity collision)
        transform.position += Vector3.right * (isLeft ? -0.01f : 0.01f);
        isLeft = !isLeft;

        // Regulate TP process
        // If IncideTPTimer > 0 than AI now in TP..
        if(IncideTPTimer > 0f)
        {
            // So we need count down timer, and if we run out of time..
            IncideTPTimer -= Time.deltaTime;
            if(IncideTPTimer < 0f)
            {
                // TP ends
                IncideTPTimer = 0f;
                EndOfTP();
            }
        }

        
        // Basic AI_State regulation
        if (EnemyTriggered && enemy != null)
        {
            
            List<RaycastHit2D> MyRayCast;
            // Does we see enemy?
            // First of all does we see at least something (if false ray didn't hit any visible for bot object)
            if (SoftRayCast2D(transform.position, enemy.transform.position, out MyRayCast))
            {
                // If we are not attack but we clearly see player, we are going to attack him
                if (MyState < AI_State.Attack && MyRayCast[MyRayCast.Count - 1].transform.tag == "Player")
                {
                    GoAttack(MyRayCast[MyRayCast.Count - 1].transform.gameObject);
                }

                // On the oposite side if we attacking someone, but we no longer see him, we going to search
                if(MyState == AI_State.Attack && MyRayCast[MyRayCast.Count - 1].transform.tag != "Player")
                {
                    GoSearch();
                }
            }
            
        }

        // If enemy which we attack or search for die (or something else) we going to Idle
        if (enemy == null && MyState != AI_State.Idle)
            GoIdle();

        TPTimer -= Time.deltaTime;





        // Here we managing AI based on AI_State

        // If we idle we just serching and tp to points around center point
        if (MyState == AI_State.Idle)
        {

            if(TPTimer<0f)
            {
                if (IdleSpotGenerator.FindSpot(out Location))
                {
                    TP_ToPos(Location);
                }
                TPTimer = TPRate * 1.5f;
            }
        }

        // If we attacking..
        if (MyState == AI_State.Attack)
        {
            // Moving aim

            EnemyAimPoint = Vector3.Lerp(EnemyAimPoint, enemy.transform.position, Time.deltaTime * AimInterpSpeed);

            // We want know when to fire
            FireTimer -= Time.deltaTime;
            if(FireTimer < 0)
            {
                // But if we in TP now we want delay fire
                if (IncideTPTimer == 0f)
                {
                    Fire(EnemyAimPoint);
                    FireTimer = FireRate;
                }
            }

            // We want know when to TP
            if(TPTimer < 0)
            {
                Vector3 PosToTP;
                // We not TP if firespotgenerator didn't found point for us
                if (FireSpotGenerator.FindSpot(out PosToTP))
                    TP_ToPos(PosToTP);

                TPTimer = TPRate;
            }
        }

        // If we are serching.. 
        // (you will see that on serch we use half of TPRate, so on search TP faster)
        if(MyState == AI_State.Search)
        {
            // first of all we need to know on which search state we are
            switch (MySearchState)
            {
                // On first state we are going to loc, where player was when he disapiar
                case SearchState.ToLastLoc:
                    {
                        if (TPTimer < TPRate / 2)
                        {
                            if (TP_ToPos(Location))
                            {
                                // When we reach loc we are finding loc where player could went
                                if (SearchSpotGenerator.FindSpot(out Location))
                                    MySearchState = SearchState.ToNewLoc;
                                else
                                {
                                    // if we didn't find loc we just going to wait
                                    MySearchState = SearchState.Waiting;
                                    SearchWaitTimer = SearchWaitingTime;
                                }
                            }
                            TPTimer = TPRate;
                        }
                        break;
                    }
                    // Now we going to theoretical place player could went..
                case SearchState.ToNewLoc:
                    {
                        if(TPTimer < TPRate / 2)
                        {
                            // And when we reach this place
                            if(TP_ToPos(Location))
                            {
                                // We are going to wait in this place
                                MySearchState = SearchState.Waiting;
                                TPTimer = TPRate;
                                SearchWaitTimer = SearchWaitingTime;
                            }
                            TPTimer = TPRate;
                        }
                        break;
                    }
                    // Just waiting..
                case SearchState.Waiting:
                    {
                        SearchWaitTimer -= Time.deltaTime;
                        if (SearchWaitTimer < 0)
                            GoIdle();
                        break;
                    }
                    // If something went wrong just go Idle
                default:
                    {
                        GoIdle();
                        break;
                    }
            }
        }



    }
    // When player enter perception
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            enemy = other.gameObject;
            EnemyTriggered = true;
        }
    }

    // When player exit perception
    void OnTriggerExit2D(Collider2D other)
    {
        // Make sure is this exact our player
        if (other.tag == "Player" && enemy == other.gameObject) 
        {
            // If we idle nothing interesting for us, and if we serching we can't serch again
            if (MyState == AI_State.Attack)
                GoSearch();

            EnemyTriggered = false;
        }
    }

    // on idle we..
    public void GoIdle()
    {
        // if you need to explaine this, you are dumb
        MyState = AI_State.Idle;
        Perception.radius = ViewRadius;

        // Because of simplicity of AI, it cant walk back on point where it spawned, so it will walk around point where it become Idle
        IdleCenter = transform.position;

        TPTimer = 1.5f * TPRate;
    }

    // on search we..
    public void GoSearch()
    {
        // seting some locations
        Location = enemy.GetComponent<CharMovement>().MyLastLoc;
        MyLastLoc = transform.position;
        
        // recharge serch states
        MyState = AI_State.Search;
        MySearchState = SearchState.ToLastLoc;

        // and recharge TP so we can run as soon as we loose player
        TPTimer = TPRate;
    }

    // on attacking we..
    public void GoAttack(GameObject Enemy)
    {
        // seting enemy
        enemy = Enemy;

        // Every time we find enemy we want aim directly into it
        EnemyAimPoint = enemy.transform.position;

        // if we was Idle our perseption was small, so we need expand it
        if(MyState == AI_State.Idle)
            Perception.radius = ViewRadius * 1.5f;

        MyState = AI_State.Attack;

        // just becouse
        FireTimer = FireRate / 2;
        
    }

    // while we in tp we invisible and has no collision, here we returning that back
    private void EndOfTP()
    {
        gameObject.GetComponent<SpriteRenderer>().enabled = true;

        gameObject.layer = 11;
    }

    // Siply fire
    public void Fire(Vector3 InPoint)
    {

        // If bullet only one, just fire it
        if (NumOfBullet < 2)
        {
            FireBullet(InPoint - transform.position);
            return;
        }

        // first of all we need find start rotation
        float StartZRotation = Quaternion.FromToRotation(Vector3.up, (InPoint - transform.position)).eulerAngles.z - (NumOfBullet - 1) * DeltaAngleOfFire / 2f;

        float NextRot;
        Vector3 DirectionToSpawn = new Vector3();

        // after that we find every new rotation and spawned bullet
        for (int i = 0; i < NumOfBullet; ++i)
        {
            NextRot = StartZRotation + DeltaAngleOfFire * i;

            if (NextRot < 0)
                NextRot += 360;
            
            DirectionToSpawn.x = Mathf.Cos(Mathf.PI * (NextRot + 90f) / 180f);
            DirectionToSpawn.y = Mathf.Sin(Mathf.PI * (NextRot + 90f) / 180f);

            FireBullet(DirectionToSpawn);
        }
    }

    // Simply fire bullet
    private void FireBullet(Vector3 InDirection)
    {
        // finding rotation        
        Quaternion SpawnQuat = Quaternion.FromToRotation(Vector3.up, InDirection);

        GameObject MyBullet = null;
        
        // We spawn bullet itself..
        if (Bullet != null)
            MyBullet = Instantiate(Bullet, transform.position + InDirection.normalized * 2, SpawnQuat);
        else
        {
            print("Out of BulletToShoot (CharShooting)");
            return;
        }

        // and if we are not loosing anything we seting bullet values
        if (MyBullet != null)
        {
            
            MyBullet.GetComponent<Bullet>().SetLoadLevel(BulletLoadLevel);
            MyBullet.GetComponent<Bullet>().Tag = "Player";
            MyBullet.layer = 9;
        }
    }

    // TP us to some position
    public bool TP_ToPos(Vector3 InPoint)
    {
        // first of all we spawn particle
        if (TP_Particle != null)
        {
            GameObject MyPar = Instantiate(TP_Particle, transform.position, transform.rotation);

            // For particle attract to our finale position
            ParticleAttractor MyAttractor = MyPar.GetComponent<ParticleAttractor>();
            if (MyAttractor != null)
                MyAttractor.PointToAttract = transform;
        }
        
        // Make us invisible
        GetComponent<SpriteRenderer>().enabled = false;
        // recharge timer
        IncideTPTimer = 0.5f;
        // Make us invincible for bullets
        gameObject.layer = 15;

        // if distance allows us to tp in exact point we will return true
        bool ToReturn = false;
        Vector2 InPosLocal = InPoint;

        // if distance more then tp distance we we will tp only on distance of tp
        if ((InPoint - transform.position).magnitude > TPDistance)
            InPosLocal = transform.position + (InPoint - transform.position).normalized * TPDistance;
        else
            ToReturn = true;
        
        // In this section we check if we colliding with something..

        RaycastHit2D[] NewRaycastHit;

        int Num = SphereHit(out NewRaycastHit, InPoint, 2);

        // and if yes we adjust location (correcting InPosLocal)
        if (Num > 0)
        {
            float Coeffition;
            Vector2 MyVector;
            
            // we are checking for every hit
            for (int i = 0; i < Num; ++i)
            {
                
                MyVector = InPosLocal - NewRaycastHit[i].point;

                // find on which distance from the wall we need to adjust our location 
                Coeffition = Mathf.Abs(MyVector.x * NewRaycastHit[i].normal.x + MyVector.y * NewRaycastHit[i].normal.y);

                // and adjust it
                InPosLocal += NewRaycastHit[i].normal * (2 - Coeffition);
            }
            
        }


        // After all tp us to that position
        transform.position = InPosLocal;

        return ToReturn;

    }


    // Basicly for adjusting location on tp
    private int SphereHit(out RaycastHit2D[] Res, Vector2 Point, float Radius)
    {
        // We are making 8 raycasts in all directions (by rotating them)
        
        int RetInt = 0;

        Vector2 ToVector = new Vector2();

        Res = new RaycastHit2D[8];

        for (int i = 0; i < 8; ++i)
        {
            if (Res.Length == RetInt)
                break;
            
            ToVector.x = Mathf.Cos(Mathf.PI / 4f * i);
            ToVector.y = Mathf.Sin(Mathf.PI / 4f * i);
            ToVector.Normalize();
            ToVector *= Radius;
            RaycastHit2D NewHit;
            NewHit = Physics2D.Raycast(Point, ToVector, Radius);

            // and if some of them hit something we wrote it in our answer
            if (NewHit && NewHit.transform.gameObject != gameObject)
            {
                Res[RetInt] = NewHit;
                ++RetInt;
            }
            
        }
        // by the end we return number of hits
        return RetInt;
    }
    
    /* Here we basicly will skip some layers in diraction of raycast 
     * if these layers are invisible for us */
    private bool SoftRayCast2D(Vector2 Start, Vector2 End, out List<RaycastHit2D> OutHit)
    {
        OutHit = new List<RaycastHit2D>();

        RaycastHit2D LocalRayHit;

        Vector2 StartPos = Start;

        /* while we hiting "invisible" layers or didn't reach finale point
         * we adjust a liitle bit in direction of our raycast (because where will be bug with bullets ("Fuck you" - Unity5))
         * and raycasting again */
        while (true)
        {
            LocalRayHit = Physics2D.Raycast(StartPos, End - StartPos, (End - StartPos).magnitude);

            if (!LocalRayHit)
                return false;

            // every hit must be added to resoult
            OutHit.Add(LocalRayHit);
            
            
            bool isFind = false;

            // does our raycast hit an invisible layer?
            for(int i = 0; i < InvisibleLayers.Length; ++i)
            {
                if(LocalRayHit.collider.gameObject.layer == InvisibleLayers[i])
                {
                    // if yes we no longer need to check that
                    isFind = true;
                    break;
                }
            }

            // and if we hit a visible layer
            if (!isFind)
                return true;

            // if we hit invisible layer adjust location a liitle bit and go on
            StartPos = LocalRayHit.point + (LocalRayHit.point - StartPos).normalized * 0.01f;

            // just check maby something wrong (by that I found bug with bullet*)
            if (OutHit.Count > 30)
            {
                print("Too much Hits!!!");
                return false;
            }
        }

    }

}


/* Bug with bullet
 * so basicly if raycast will hit moving object, which moving in direction of raycast,
 * raycast will gave hit location of object in previous frame.
 * So if by my "SoftCast" you will fire through few invisible walls,
 * by the end it will gave hits for every wall it went through.
 * Only one hit for each wall, even without adjusting location by
 * "(LocalRayHit.point - StartPos).normalized * 0.01f".
 * It all works fine.
 * But if you fire by "SoftRay" in bullet, which go in the same way,
 * you will have more then 30 hit of that same bullet (I don't how many exactly hits
 * becouse i had this check for 30 hits, and it paid off).
 * Even WITH adjusting location it still gave not 1 but 5 hits of the same bullet.
 * So it's like Unity say to us "Fuck you, my dear community)" */ 
