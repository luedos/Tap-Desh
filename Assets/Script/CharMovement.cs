using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharMovement : MonoBehaviour {

    public float CameraInterpSpeed = 0;


    private float BetterTPTimer = 0f;
    public bool isBetterTP = false;
    private Camera MyCamera;
    private MobileInput MyMobileInput;
    private Vector3 CameraPosition;


    // Use this for initialization
    void Start () {
        MyMobileInput = GetComponent<MobileInput>();
        if (MyMobileInput == null)
            print("Out of mobile input (CharMovement)");



        MyCamera = GetComponentInChildren<Camera>();

        if (MyCamera == null)
            print("Out of camera (CharMovement)");

        CameraPosition = transform.position;
        CameraPosition.z = 5f;
        MyCamera.transform.position = CameraPosition;
    }
	
	// Update is called once per frame
	void Update () {
		
       if(isBetterTP)
       {
           BetterTPTimer -= Time.deltaTime;
           if(BetterTPTimer < 0)
           {
               BetterTPTimer = 0;
               isBetterTP = false;
           }
       }

        if(MyMobileInput.Tap)
        {
            GoToPosition( GetPositionFromScreen(MyMobileInput.LastPosition));
        }

        float ToInterp = CameraInterpSpeed * Time.deltaTime;

        if (ToInterp > 1 || CameraInterpSpeed == 0)
            CameraPosition = transform.position;
        else
           CameraPosition = Vector3.Lerp( CameraPosition, transform.position, ToInterp);

        CameraPosition.z = 5;

        MyCamera.transform.position = CameraPosition;
    }

    

    private Vector3 GetPositionFromScreen(Vector2 InScreenPos)
    {
        Vector3 MyVector = MyCamera.ScreenToWorldPoint(new Vector3(InScreenPos.x, InScreenPos.y, 0));
        MyVector.z = 0;
        
        return MyVector;
    }

    private bool GoToPosition(Vector2 InPosition)
    {
        RaycastHit2D NewHit;
        Vector2 MyOrigin = transform.position;
        Vector2 InPosLocal;

        
        if (isBetterTP)
        {
            List<RaycastHit2D> NewList;

            bool CollRes = true;

            Collider2D MyColl = Physics2D.OverlapPoint(InPosition);

            if (MyColl != null)
                if (MyColl.tag != transform.tag)
                    CollRes = false;

           

            if (!SoftRayCast2D(MyOrigin, InPosition, out NewList) && CollRes)
                InPosLocal = InPosition;
            else
                InPosLocal = NewList[NewList.Count - 1].point + NewList[NewList.Count - 1].normal * 0.1f;
           

            

        }
        else
        { 
            NewHit = Physics2D.Raycast(MyOrigin, InPosition - MyOrigin, (InPosition - MyOrigin).magnitude);
            InPosLocal = NewHit ? NewHit.point + NewHit.normal * 0.1f : InPosition;
        }
        RaycastHit2D[] NewRaycastHit = new RaycastHit2D[8];

        int Num = SphereHit(ref NewRaycastHit, InPosLocal, 2);

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

            transform.position = InPosLocal;

            return true;
        }
        

        

        transform.position = InPosLocal;

        return true;
      
    }

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


    public void MakeBetterTP(float InTime)
    {
        isBetterTP = true;
        BetterTPTimer = InTime;
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
            

            if (LocalRayHit.collider.gameObject.layer != 14)
                return true;

            StartPos = LocalRayHit.point;


            if (OutHit.Count > 30)
            {
                print("Too much Hits!!!");
                return false;
            }
        }

    }
}


