using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharMovement : MonoBehaviour {

    public float CameraInterpSpeed = 0;

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
        Vector2 MyOrigin = transform.position;
        

        RaycastHit2D NewHit;
        NewHit = Physics2D.Raycast(MyOrigin, InPosition-MyOrigin,(InPosition-MyOrigin).magnitude);
       
        Vector2 InPosLocal = NewHit ? NewHit.point + NewHit.normal * 0.1f : InPosition;

        RaycastHit2D[] NewRaycastHit = new RaycastHit2D[8];

        int Num = SphereHit(ref NewRaycastHit, InPosLocal, 2);

        if (Num > 0)
        {
            print("Good num");
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
        
        transform.position = InPosition;

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
}


