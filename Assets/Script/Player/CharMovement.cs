using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof( MobileInput))]
public class CharMovement : MonoBehaviour {

    public float CameraInterpSpeed = 0;         // How fust camera will move(interp) from one point to another
    public GameObject MyParticle;               // particle on spawn
    public GameObject EndTP_Particle;           
    public float TPTime = 0.5f;                 // How long tp will last by itself
    public float TPPenalty = 0.5f;

    public int[] WalkThroughLayers;
    public int[] TP_InLayers;

    [HideInInspector]
    public Vector3 MyLastLoc;                   // basicly this one used by AI

    private float BetterTPTimer = 0f;
    private bool isBetterTP = false;
    private Camera MyCamera;                    // Camera, to find point from the screen and interp it
    private MobileInput MyMobileInput;          // mobile input to get know when and where to tp
    private Vector3 CameraPosition;             // using for interpolate camera position (basicly just local variable)
    private float TPPenaltyTimer = 0f;

    private float VisibleTimer = 0f;            // timer for count time in TP itself


    //private bool isLeft = false;

    // Checking if we have everything and filling some variables
    void Start () {
        MyMobileInput = GetComponent<MobileInput>();
        if (MyMobileInput == null)
            print("Out of mobile input (CharMovement)");

        MyLastLoc = transform.position;

        MyCamera = GetComponentInChildren<Camera>();

        if (MyCamera == null)
            print("Out of camera (CharMovement)");

        CameraPosition = transform.position;
        CameraPosition.z = -5f;
        MyCamera.transform.position = CameraPosition;


    }
	
	// Update is called once per frame
	void Update () {

        // transform.position += Vector3.right * (isLeft ? -0.01f : 0.01f);
        // isLeft = !isLeft;

        // Is we in TP now
        if (VisibleTimer > 0f)
        {
            // if yes just reduse timer
            VisibleTimer -= Time.deltaTime;

        }
        else
        if (VisibleTimer < 0f)
        {
            // but if we was in tp frame before and now it end we set timer in 0 (so first and second "if" will give false), and return us visible 
            VisibleTimer = 0f;
            ReturnVisible();
        }

        if (TPPenaltyTimer > 0f)
            TPPenaltyTimer -= Time.deltaTime;
        else if (TPPenaltyTimer < 0f)
            TPPenaltyTimer = 0f;

        
       // if we in beter tp, reduse timer of it
        if (isBetterTP)
       {
           BetterTPTimer -= Time.deltaTime;
           if(BetterTPTimer < 0f)
           {
               BetterTPTimer = 0f;
               isBetterTP = false;
           }
       }

        // if we has tap, then we want to tp..
        if (MyMobileInput.Tap && TPPenaltyTimer == 0f)
        {
            
            MyLastLoc = transform.position;
            GoToPosition( GetPositionFromScreen(MyMobileInput.LastPosition));
        }

       // interpolation of camera position to our position
        float ToInterp = CameraInterpSpeed * Time.deltaTime;
       
        if (ToInterp > 1 || CameraInterpSpeed == 0)
            CameraPosition = transform.position;
        else
           CameraPosition = Vector3.Lerp( CameraPosition, transform.position, ToInterp);

        CameraPosition.z = -5;
        MyCamera.transform.position = CameraPosition;
    }

    
    // because mobile inpup give us not location but point on the screen in pixels we need to transform it in actual position in world
    private Vector3 GetPositionFromScreen(Vector2 InScreenPos)
    {
        Vector3 MyVector = MyCamera.ScreenToWorldPoint(new Vector3(InScreenPos.x, InScreenPos.y, 0));
        MyVector.z = 0;
        
        return MyVector;
    }

    // just set all variables back after end of tp
    private void ReturnVisible()
    {

        gameObject.GetComponent<SpriteRenderer>().enabled = true;

        if (EndTP_Particle != null)
            Instantiate(EndTP_Particle, transform.position, transform.rotation);

        gameObject.layer = 12;
    }

    // so I have a lot of explain here. 
    private bool GoToPosition(Vector2 InPosition)
    {

        
        Vector2 MyOrigin = transform.position;
        Vector2 InPosLocal;                         // this thing we need becouse we would adjust location on tp


        // here we just spawn particle
        if(MyParticle != null)
        {
            GameObject MyPar = Instantiate(MyParticle,transform.position,transform.rotation);
            
            // because of specific particle, we need set attractor
            ParticleAttractor MyAttractor = MyPar.GetComponent<ParticleAttractor>();
            if (MyAttractor != null)
            {
                MyAttractor.PointToAttract = transform;
            }
        }



        // this how these variable need to act when we in tp
        GetComponent<SpriteRenderer>().enabled = false;
        gameObject.layer = 15;
        // and reset tp timer
        VisibleTimer = TPTime;
        TPPenaltyTimer = TPPenalty;



        // first of all we need to check is our possible finale location in something 
        bool CollRes = IsValidPoint(InPosition);


        // after that we need to check soft ray (if it will gave false we will tp into strong wall)
        List<RaycastHit2D> NewList;

        // if coll res is true (not pointing in anything) and raycast gave false (so we didn't cross any strong wall or somethiing) we just tp in this position
        if (!SoftRayCast2D(MyOrigin, InPosition, out NewList) && CollRes)
            InPosLocal = InPosition;
        else
        {
            InPosLocal = NewList[NewList.Count - 1].point + NewList[NewList.Count - 1].normal * 0.2f; // but if we was pointing into something, or we cross some strong wall, we mast take last hit from raycast

            //InPosLocal = InPosLocal + ((Vector2)transform.position - InPosLocal).normalized * 0.2f;
        }

        // after we tp we want to adjust our location if possible 

        RaycastHit2D[] NewRaycastHit = new RaycastHit2D[8];

        int Num = SphereHit(ref NewRaycastHit, InPosLocal, 2);

        // if Num == 0, it's meen we didn't hit anything and we did not need to adjust collision
        if (Num > 0)
        {
            
            float Coeffition;
            Vector2 MyVector;


            // from every normal of each hit we have, we adjusting location on distance we need to not colliding with this wall (or some thing)
            // P.S. all this system of adjusting very simplified and works only with circles
            for (int i = 0; i < Num; ++i)
            {
               
                MyVector = InPosLocal - NewRaycastHit[i].point;

                Coeffition = Mathf.Abs(MyVector.x * NewRaycastHit[i].normal.x + MyVector.y * NewRaycastHit[i].normal.y);

                InPosLocal += NewRaycastHit[i].normal * (2 - Coeffition);
            }

        }


        // after all we just set our position

        transform.position = InPosLocal;

        return true;

    }

    // all this do is make 8 raycast in 360 deg., and record result in "Res"
    private int SphereHit(ref RaycastHit2D[] Res, Vector2 Point, float Radius)
    {
        int RetInt = 0;

        Vector2 ToVector = new Vector2();
        


        for(int i = 0; i < 8; ++i)
        {
            if (Res.Length == RetInt)
                break;


            ToVector.x = Mathf.Cos(Mathf.PI / 4f * i);
            ToVector.y = Mathf.Sin(Mathf.PI / 4f * i);
            ToVector.Normalize();
            ToVector *= Radius;
            RaycastHit2D NewHit;
            NewHit = Physics2D.Raycast(Point, ToVector, Radius);

            if(NewHit && NewHit.transform.gameObject != gameObject)
            {                
                Res[RetInt] = NewHit; 
                ++RetInt;
            }

            
            
        }


        return RetInt;
    }

    // just seting better tp
    public void MakeBetterTP(float InTime)
    {
        if(InTime == 0f)
        {
            if(isBetterTP)
            {
                isBetterTP = false;
                BetterTPTimer = 0f;
                GameManager.Instance.myHUD.GetComponent<HUDScript>().AddBonusIcon_BT(0f);

            }

            return;
        }

        isBetterTP = true;
        BetterTPTimer = InTime;
        GameManager.Instance.myHUD.GetComponent<HUDScript>().AddBonusIcon_BT(InTime);
    }


    // raycast which can overlap "soft" walls
    private bool SoftRayCast2D(Vector2 Start, Vector2 End, out List<RaycastHit2D> OutHit)
    {
       
        // seting up some variables
        OutHit = new List<RaycastHit2D>();

        RaycastHit2D LocalRayHit;

        Vector2 StartPos = Start;
  
        
        // we break only if.. 
        while (true)
        {
            LocalRayHit = Physics2D.Raycast(StartPos, End - StartPos, (End - StartPos).magnitude);

            // raycast reach finale destination
            if (!LocalRayHit)
                return false;

            // ofcourse every hit must be in result
            OutHit.Add(LocalRayHit);

            // or we hit not soft wall
            bool findLayer = false;


            if (isBetterTP && LocalRayHit.transform.gameObject.layer == 14)
                findLayer = true;
            else
                for (int i = 0; i < WalkThroughLayers.Length; ++i)
                {
                    if (WalkThroughLayers[i] == LocalRayHit.transform.gameObject.layer)
                    {
                        findLayer = true;
                        break;
                    }
                }

            if (!findLayer)
                return true;

            // relocate start pos so next ray start incide soft wall
            StartPos = LocalRayHit.point + (End - Start).normalized * 0.15f;

            // and in the end check for some bugs (what if something will go wrong)
            if (OutHit.Count > 30)
            {
                print("Too much Hits!!!");
                return false;
            }
        }

    }

    bool IsValidPoint(Vector2 InCoord)
    {
        Collider2D MyColl = Physics2D.OverlapPoint(InCoord);

        if (MyColl == null)
            return true;

        if (MyColl.tag == gameObject.tag)
            return true;

        for(int i = 0; i < TP_InLayers.Length; ++i)
        {
            if (MyColl.gameObject.layer == TP_InLayers[i])
                return true;
        }

        return false;
    }
}


