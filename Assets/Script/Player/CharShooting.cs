using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharShooting : CharPart {


    public GameObject BulletToShoot;
    public float MaxLoad = 2;                   // Max lvl we can load our bullet by holding touch on character
    public float TouchSensitivity = 0f;         // in what radius from char we can tap so load will start (if = 0, you can touch in every radius)
    public float LoadMultiplier = 1f;           // Load level per second
    public float MultiFireAngle = 20f;          // angle between bullets (if where more then one will fire)

    private bool IsDD = false;                  // is double damage
    private float DDTimer = 0f;  
    private Camera MyCamera;                    // Camera, to find point from the screen 
    private MobileInput MyMobileInput;          // mobile input to get know when and where to tp
    private bool LoadChar = false;              // is we loading boolet
    private float Load = 0f;                    // power of load on the moment
    private int FireMultiplacator = 0;          // it's level of how many bullets will be on sides

    public int GetFireMode { get { return FireMultiplacator; } }

    // simple check is every thing ok
    void Start () {
        MyMobileInput = GetComponent<MobileInput>();

        if (MyMobileInput == null)
            print("Out of mobile input (CharShooting)");

        MyCamera = GetComponentInChildren<Camera>();

        if (MyCamera == null)
            print("Out of camera  (CharShooting)");
    }
	
	
	void Update () {
    
        
        // simple timer for DD
        if(IsDD)
        {
            DDTimer -= Time.deltaTime;
            if(DDTimer < 0)
            {
                IsDD = false;
                DDTimer = 0;
            }
        }


        // we start to load on touch (if this touch was in radius)
        if (MyMobileInput.Touch && !LoadChar)
        {            
            LoadChar = true;
        }
        
        
        if (LoadChar)
        {

            // we increase load lvl till max
            if (Load < MaxLoad)
                Load += Time.deltaTime * LoadMultiplier;
            else if (Load > MaxLoad)
                Load = MaxLoad;

            // and change color to more red (reduse green and blue)
            GetComponent<SpriteRenderer>().color = new Color(1 + Load, (MaxLoad - Load) / MaxLoad, (MaxLoad - Load) / MaxLoad);

        }



        // We fire if we swiped or long tap
        if (LoadChar && MyMobileInput.LongTap)
        {
            
            Vector3 StartVector = GetPositionFromScreen(MyMobileInput.LastPosition)- transform.position;
            Quaternion SpawnQuat = Quaternion.FromToRotation(Vector3.up, StartVector);
            
            StartVector = StartVector.normalized * 2f + transform.position;

            // Fire first bullet
            Fire(StartVector, SpawnQuat);
            
            // and if multiplacator more than one we spawn rest
            if (FireMultiplacator > 0)
            {
                // by this furmula we finding exact number of bullets on sides of fire direction
                int ForLength = FireMultiplacator > 2 ? FireMultiplacator * 4 - 4 : FireMultiplacator * 2;

                // basicly works like ping pong, spawning bullets from center (just accept it)
                for (int i = 1; i < ForLength + 1; ++i)
                {
                    float SpawnAngleZ = Quaternion.FromToRotation(Vector3.up, StartVector - transform.position).eulerAngles.z;

                    SpawnAngleZ += MultiFireAngle * (i % 2 == 0 ? (-1f) : 1f) * ((i + 1) / 2);

                    if (SpawnAngleZ < 0)
                        SpawnAngleZ += 360;

                    Vector3 VectorToSpawn = new Vector3();

                    VectorToSpawn.x = Mathf.Cos(Mathf.PI * (SpawnAngleZ + 90f) / 180f);
                    VectorToSpawn.y = Mathf.Sin(Mathf.PI * (SpawnAngleZ + 90f) / 180f);

                    Fire(VectorToSpawn * 2f + transform.position, Quaternion.Euler(0, 0, SpawnAngleZ));


                }
            }

            
            ResetAll();
        }

        // Tap meen it's not swipe, so we didn't shoot, but start to load, so we want reset everything
        // about "!MyMobileInput.IsDraging && LoadChar" that meen what we are not draging, but still loading somewhy, because of that we want reset all
        if ((!MyMobileInput.IsDraging && LoadChar))
        {
            ResetAll();
        }
    }

    public override void ResetPart()
    {
        IsDD = false;
        DDTimer = 0f;

        FireMultiplacator = 0;
    }

    // because mobile inpup give us not location but point on the screen in pixels we need to transform it in actual position in world
    private Vector3 GetPositionFromScreen(Vector2 InScreenPos)
    {
        Vector3 MyVector = MyCamera.ScreenToWorldPoint(new Vector3(InScreenPos.x, InScreenPos.y, 0));
        MyVector.z = 0;

        return MyVector;
    }

    // just reseting all needed variables after we shoot
    void ResetAll()
    {
        Load = 0f;
        LoadChar = false;
        
        GetComponent<SpriteRenderer>().color = Color.white;
    }

    // set up DD
    public void MakeDD(float InTime)
    {
        if (InTime == 0f)
        {
            if (IsDD)
            {
                DDTimer = 0f;
                IsDD = false;
                GameManager.Instance.myHUD.GetComponent<HUDScript>().AddBonusIcon_DD(0f);
                
            }
            return;
        }

        IsDD = true;
        DDTimer = InTime;
        GameManager.Instance.myHUD.GetComponent<HUDScript>().AddBonusIcon_DD(InTime);
    }

    // simple fire one bullet
    private void Fire(Vector3 SpawnVector, Quaternion SpawnQuat)
    {

        GameObject MyBullet = null;

        if (BulletToShoot != null)
            MyBullet = Instantiate(BulletToShoot, SpawnVector, SpawnQuat);
        else
            print("Out of BulletToShoot (CharShooting)");

        if (MyBullet != null)
        {
            if (IsDD)
                MyBullet.GetComponent<Bullet>().BulletSpeed *= 2;

            float LoadLvl = IsDD ? (1 + Load) * 2 : 1 + Load;
            MyBullet.GetComponent<Bullet>().SetLoadLevel(LoadLvl);
            MyBullet.GetComponent<Bullet>().Tag = "Enemy";
            MyBullet.layer = 10;
        }
    }

    // here we can change fire multiplicator by some rules
    public void SetFireMultOnLevel(int InLevel)
    {
        switch (InLevel)
        {
            case -1:
                {
                    if (FireMultiplacator > 0)
                    {
                        --FireMultiplacator;                        
                    }
                    break;
                }
            case 5:
                {
                    if (FireMultiplacator < 5)
                    {                        
                        ++FireMultiplacator;
                    }
                    break;
                }

            default:
                {
                    if (InLevel > -1 && InLevel < 5)
                        FireMultiplacator = InLevel;
                    break;
                }
        }
    }

}
