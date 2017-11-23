using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileInput : MonoBehaviour {

    public float SwipeSensitivity = 125f;
    public float TapTimeSensitivity = 0.2f;
    public float TapRadialSensitivity = 40f;

    private Camera MyCamera;
    private bool tap, touch, swipe, turn;
    private Vector2 startTouch, deltaTouch, lastPosition;
    private bool isDraging = false;
    private float TapTimer = 0f;
    private int LastTurnIndex = 0;
    private Vector3 StartTurnVector;
    private Vector2 CenterTurnVector;

    public Vector2 StartPosition { get { return startTouch; } }
    public Vector2 DeltaTouch { get { return deltaTouch; } }
    public Vector2 LastPosition { get { return lastPosition; } }
    public bool Tap { get { return tap; } }
    public bool Touch { get { return touch; } }
    public bool Swipe { get { return swipe; } }
    public bool Turn { get { return turn; } }
    public bool IsDraging { get { return isDraging; } }

    public void Start()
    {
        MyCamera = GetComponentInChildren<Camera>();

        if (MyCamera == null)
            print("Out of camera  (Mobile Input)");
    }

    private void Reset()
    {
        TapTimer = 0f;
        isDraging = false;
        startTouch = deltaTouch = Vector2.zero;
        LastTurnIndex = 0;
    }

	// Update is called once per frame
	void Update ()
    {
        tap = touch = swipe = turn = false;


        // for tap (if we relese finger/mouse and timer is not run out this is Tap)
        if (isDraging)
            TapTimer += Time.deltaTime;


        // Input for mouse
        if(Input.GetMouseButtonDown(0))
        {
            startTouch = Input.mousePosition;
            touch = true;
            isDraging = true;

            CenterTurnVector = MyCamera.WorldToScreenPoint(transform.position);
          
            StartTurnVector = Quaternion.FromToRotation(Vector3.up, (startTouch - CenterTurnVector)).eulerAngles;
        }
        else if(Input.GetMouseButtonUp(0))
        {
            lastPosition = Input.mousePosition;

            if (TapTimer < TapTimeSensitivity && isDraging && (lastPosition - startTouch).magnitude < TapRadialSensitivity)
                tap = true;


            


            Reset();
        }

        // Input for TouchScreen
        if(Input.touches.Length > 0)
        {
            if(Input.touches[0].phase == TouchPhase.Began)
            {
                startTouch = Input.touches[0].position;
                touch = true;
                isDraging = true;

                CenterTurnVector = MyCamera.WorldToScreenPoint(transform.position);

                StartTurnVector = Quaternion.FromToRotation(Vector3.up, (startTouch - CenterTurnVector)).eulerAngles;
            }
            else if(Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled)
            {
                if (TapTimer < TapTimeSensitivity && isDraging)
                    tap = true;

                lastPosition = Input.touches[0].position;

              

                Reset();
            }
        }

        // If we draging we seting delta touch
        if(isDraging)
        {
            if (Input.touches.Length > 0)
                deltaTouch = Input.touches[0].position - startTouch;
            else if (Input.GetMouseButton(0))
                deltaTouch = (Vector2)Input.mousePosition - startTouch;

        }

        // Check if swipe
        if(deltaTouch.magnitude > SwipeSensitivity)
        {
            swipe = true;

            lastPosition = deltaTouch + startTouch;
            
            Reset();
        }

        if(isDraging)
        {
            float NowTurn = Quaternion.FromToRotation(Vector3.up, (startTouch + deltaTouch - CenterTurnVector)).eulerAngles.z - StartTurnVector.z;
            if (NowTurn < 0)
                NowTurn += 360f;

            int NowIndex = Mathf.FloorToInt(Mathf.Floor(NowTurn / 45f));

            if(NowIndex == 0 && LastTurnIndex == 7)
            {
                print("Turn!");
                turn = true;
                Reset();
            }

            if (Mathf.Abs(NowIndex - LastTurnIndex) == 1)
                LastTurnIndex = NowIndex;
        }
	}


    private Vector3 GetPositionFromScreen(Vector2 InScreenPos)
    {
        Vector3 MyVector = MyCamera.ScreenToWorldPoint(new Vector3(InScreenPos.x, InScreenPos.y, 0));
        MyVector.z = 0;

        return MyVector;
    }



}
