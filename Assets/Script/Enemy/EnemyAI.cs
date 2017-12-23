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
    private bool EnemyTriggered = false;
    private float TPTimer = 0f;
    private float FireTimer = 0f;
    private Vector3 Location;
    private float ViewRadius = 20f;
    // For search method
    private Vector3 MyLastLoc;
    private SearchState MySearchState = 0;
    private float SearchWaitTimer;
    private float IncideTPTimer = 0f;

    // For Enemy customisation    
    public int[] InvisibleLayers;
    public GameObject Bullet = null;
    public float FireRate = 1.5f;
    public float TPDistance = 5f;
    public float TPRate = 1.5f;
    public CircleCollider2D Perception;
    public U_EQS_Generator FireSpotGenerator = null;
    public U_EQS_Generator SearchSpotGenerator = null;
    public float BulletLoadLevel = 1f;
    public float SearchWaitingTime = 3f;
    public float TPTime = 0.5f;
    public GameObject TP_Particle = null;

    public Vector3 StartSerchLocation { get { return MyLastLoc; } }
    public Vector3 Destination { get { return Location; } }
    public GameObject Enemy { get { return enemy; } }
	// Use this for initialization
	void Start () {

        

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

        FireTimer = FireRate;
        TPTimer = TPRate;
        SearchWaitTimer = SearchWaitingTime;
    }
	
	// Update is called once per frame
	void Update () {


        switch (MyState)
        {
            case AI_State.Idle:
                {
                    GetComponent<SpriteRenderer>().color = Color.green;
                    break;
                }
            case AI_State.Search:
                {
                    GetComponent<SpriteRenderer>().color = Color.yellow;
                    break;
                }
            case AI_State.Attack:
                {
                    GetComponent<SpriteRenderer>().color = Color.red;
                    break;
                }
            default:
                break;
        }

        
        // For make collision allways work ("fuck you" - unity collision)
        transform.position += Vector3.right * (isLeft ? -0.01f : 0.01f);
        isLeft = !isLeft;


        if(IncideTPTimer > 0f)
        {
            IncideTPTimer -= Time.deltaTime;
            if(IncideTPTimer < 0f)
            {
                IncideTPTimer = 0f;
                EndOfTP();
            }
        }

        
        if (EnemyTriggered && enemy != null)
        {
            //RaycastHit2D MyRay = Physics2D.Raycast(transform.position, enemy.transform.position - transform.position, (enemy.transform.position - transform.position).magnitude);

            List<RaycastHit2D> MyRayCast;

            if (SoftRayCast2D(transform.position, enemy.transform.position, out MyRayCast))
            {
                if (MyState < AI_State.Attack && MyRayCast[MyRayCast.Count - 1].transform.tag == "Player")
                {
                    GoAttack(MyRayCast[MyRayCast.Count - 1].transform.gameObject);
                }

                if(MyState == AI_State.Attack && MyRayCast[MyRayCast.Count - 1].transform.tag != "Player")
                {
                    GoSearch();
                }
            }
            
        }

        if (enemy == null && MyState != AI_State.Idle)
            GoIdle();

        TPTimer -= Time.deltaTime;

        if (MyState == AI_State.Attack)
        {
            FireTimer -= Time.deltaTime;
            if(FireTimer < 0)
            {
                if (IncideTPTimer == 0f)
                {
                    Fire(enemy.transform.position);
                    FireTimer = FireRate;
                }
            }

            if(TPTimer < 0)
            {
                Vector3 PosToTP;
                if (FireSpotGenerator.FindSpot(out PosToTP))
                    TP_ToPos(PosToTP);

                TPTimer = TPRate;
            }
        }

        if(MyState == AI_State.Search)
        {
            switch (MySearchState)
            {
                case SearchState.ToLastLoc:
                    {
                        if (TPTimer < TPRate / 2)
                        {
                            if (TP_ToPos(Location))
                            {
                                
                                if (SearchSpotGenerator.FindSpot(out Location))
                                    MySearchState = SearchState.ToNewLoc;
                                else
                                {
                                    MySearchState = SearchState.Waiting;
                                    SearchWaitTimer = SearchWaitingTime;
                                }
                            }
                            TPTimer = TPRate;
                        }
                        break;
                    }
                case SearchState.ToNewLoc:
                    {
                        if(TPTimer < TPRate / 2)
                        {
                            if(TP_ToPos(Location))
                            {
                                MySearchState = SearchState.Waiting;
                                TPTimer = TPRate;
                                SearchWaitTimer = SearchWaitingTime;
                            }
                            TPTimer = TPRate;
                        }
                        break;
                    }
                case SearchState.Waiting:
                    {
                        SearchWaitTimer -= Time.deltaTime;
                        if (SearchWaitTimer < 0)
                            GoIdle();
                        break;
                    }
                default:
                    {
                        GoIdle();
                        break;
                    }
            }
        }



    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            enemy = other.gameObject;
            EnemyTriggered = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        
        if (other.tag == "Player" && enemy == other.gameObject) 
        {
            if (MyState == AI_State.Attack)
                GoSearch();
            

            EnemyTriggered = false;
        }
    }

    public void GoIdle()
    {
        MyState = AI_State.Idle;
        Perception.radius = ViewRadius;
    }

    public void GoSearch()
    {
        Location = enemy.GetComponent<CharMovement>().MyLastLoc;
        MyLastLoc = transform.position;
        MyState = AI_State.Search;
        MySearchState = SearchState.ToLastLoc;
        TPTimer = TPRate;
    }

    public void GoAttack(GameObject Enemy)
    {
        enemy = Enemy;
        if(MyState == AI_State.Idle)
            Perception.radius = ViewRadius * 1.5f;
        MyState = AI_State.Attack;
        FireTimer = FireRate / 2;
        
    }

    private void EndOfTP()
    {
        gameObject.GetComponent<SpriteRenderer>().enabled = true;

        gameObject.layer = 11;
    }

    public void Fire(Vector3 InPoint)
    {
        Vector3 SpawnDirection = (InPoint - transform.position).normalized;

        Quaternion SpawnQuat = Quaternion.FromToRotation(Vector3.up, SpawnDirection);

        GameObject MyBullet = null;

        if (Bullet != null)
            MyBullet = Instantiate(Bullet, transform.position + SpawnDirection * 2, SpawnQuat);
        else
            print("Out of BulletToShoot (CharShooting)");

        if (MyBullet != null)
        {
            
            MyBullet.GetComponent<Bullet>().SetLoadLevel(BulletLoadLevel);
            MyBullet.GetComponent<Bullet>().Tag = "Player";
            MyBullet.layer = 9;
        }
    }

    public bool TP_ToPos(Vector3 InPoint)
    {

        if (TP_Particle != null)
        {
            GameObject MyPar = Instantiate(TP_Particle, transform.position, transform.rotation);

            ParticleAttractor MyAttractor = MyPar.GetComponent<ParticleAttractor>();
            if (MyAttractor != null)
                MyAttractor.PointToAttract = transform;
        }
        
        GetComponent<SpriteRenderer>().enabled = false;

        IncideTPTimer = 0.5f;

        gameObject.layer = 15;

        bool ToReturn = false;
        Vector2 InPosLocal = InPoint;

        if ((InPoint - transform.position).magnitude > TPDistance)
            InPosLocal = transform.position + (InPoint - transform.position).normalized * TPDistance;
        else
            ToReturn = true;
        
        RaycastHit2D[] NewRaycastHit;

        int Num = SphereHit(out NewRaycastHit, InPoint, 2);

        if (Num > 0)
        {
            float Coeffition;
            Vector2 MyVector;

            for (int i = 0; i < Num; ++i)
            {
                MyVector = InPosLocal - NewRaycastHit[i].point;

                Coeffition = Mathf.Abs(MyVector.x * NewRaycastHit[i].normal.x + MyVector.y * NewRaycastHit[i].normal.y);

                InPosLocal += NewRaycastHit[i].normal * (2 - Coeffition);
            }
            
        }

        transform.position = InPosLocal;

        return ToReturn;

    }

    private int SphereHit(out RaycastHit2D[] Res, Vector2 Point, float Radius)
    {
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

            if (NewHit && NewHit.transform.gameObject != gameObject)
            {
                Res[RetInt] = NewHit;
                ++RetInt;
            }
            
        }
        
        return RetInt;
    }
    
    private bool SoftRayCast2D(Vector2 Start, Vector2 End, out List<RaycastHit2D> OutHit)
    {
        OutHit = new List<RaycastHit2D>();

        RaycastHit2D LocalRayHit;

        Vector2 StartPos = Start;


        while (true)
        {
            LocalRayHit = Physics2D.Raycast(StartPos, End - StartPos, (End - StartPos).magnitude);

            if (!LocalRayHit)
                return false;

            OutHit.Add(LocalRayHit);
            
            
            bool isFind = false;

            for(int i = 0; i < InvisibleLayers.Length; ++i)
            {
                if(LocalRayHit.collider.gameObject.layer == InvisibleLayers[i])
                {
                    
                    isFind = true;
                    break;
                }
            }

            if (!isFind)
                return true;

            StartPos = LocalRayHit.point + (LocalRayHit.point - StartPos).normalized * 0.01f;


            if (OutHit.Count > 30)
            {
                print("Too much Hits!!!");
                return false;
            }
        }

    }

}
