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
    // For search method
    private Vector3 MyLastLoc;
    private SearchState MySearchState = 0;
    private float SearchWaitTimer;


    // For Enemy customisation
    public float ViewRadius = 20f;
    public GameObject Bullet = null;
    public float FireRate = 1.5f;
    public float TPDistance = 5f;
    public float TPRate = 1.5f;
    public CircleCollider2D Perception;
    public U_EQS_Generator FireSpotGenerator = null;
    public U_EQS_Generator SearchSpotGenerator = null;
    public float BulletLoadLevel = 1f;
    public float SearchWaitingTime = 3f;

    public Vector3 StartSerchLocation { get { return MyLastLoc; } }
    public Vector3 Destination { get { return Location; } }
    public GameObject Enemy { get { return enemy; } }
	// Use this for initialization
	void Start () {

        if (Perception != null)
            Perception.radius = ViewRadius;
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

        
        
        if (EnemyTriggered && enemy != null)
        {
            

            RaycastHit2D MyRay = Physics2D.Raycast(transform.position, enemy.transform.position - transform.position, (enemy.transform.position - transform.position).magnitude);

            if (MyRay)
            {
                if (MyState < AI_State.Attack && MyRay.transform.tag == "Player")
                {
                    enemy = MyRay.transform.gameObject;
                    MyState = AI_State.Attack;
                    FireTimer = FireRate / 2;
                }

                if(MyState == AI_State.Attack && MyRay.transform.tag != "Player")
                {
                    Location = enemy.GetComponent<CharMovement>().MyLastLoc;
                    MyLastLoc = transform.position;
                    MyState = AI_State.Search;
                    MySearchState = SearchState.ToLastLoc;
                    TPTimer = TPRate;
                }
            }

            //if( (enemy.transform.position - transform.position).magnitude > Perception.radius)
            //{
            //    if(MyState == AI_State.Attack)
            //    {
            //        Location = enemy.GetComponent<CharMovement>().MyLastLoc;
            //        MyLastLoc = transform.position;
            //        MyState = AI_State.Search;
            //        MySearchState = SearchState.ToLastLoc;
            //        TPTimer = TPRate;
            //    }
            //                    
            //    EnemyTriggered = false;
            //}
        }

        if (enemy == null && MyState != AI_State.Idle)
            MyState = AI_State.Idle;

        TPTimer -= Time.deltaTime;

        if (MyState == AI_State.Attack)
        {
            FireTimer -= Time.deltaTime;
            if(FireTimer < 0)
            {
                Fire(enemy.transform.position);
                FireTimer = FireRate;
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
                            MyState = AI_State.Idle;
                        break;
                    }
                default:
                    {
                        MyState = AI_State.Idle;
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
            {
                Location = enemy.GetComponent<CharMovement>().MyLastLoc;
                MyLastLoc = transform.position;
                MyState = AI_State.Search;
                MySearchState = SearchState.ToLastLoc;
                TPTimer = TPRate;
            }

            EnemyTriggered = false;
        }
    }


    public void Fire(Vector3 InPoint)
    {
        Vector3 SpawnDirection = (InPoint - transform.position).normalized;

        Quaternion SpawnQuat = Quaternion.LookRotation(SpawnDirection);

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




}
