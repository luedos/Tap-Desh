using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharShooting : MonoBehaviour {


    public GameObject BulletToShoot;
    public float MaxLoad = 2;
    public float TouchSensitivity = 0f;
    public float LoadMultiplier = 1f;
    public float MultiFireAngle = 20f;

    private bool IsDD = false;
    private float DDTimer = 0f;
    private Camera MyCamera;
    private MobileInput MyMobileInput;
    private bool LoadChar = false;
    private float Load = 0f;
    private int FireMultiplacator = 0;


    // Use this for initialization
    void Start () {
        MyMobileInput = GetComponent<MobileInput>();

        if (MyMobileInput == null)
            print("Out of mobile input (CharShooting)");

        MyCamera = GetComponentInChildren<Camera>();

        if (MyCamera == null)
            print("Out of camera  (CharShooting)");
    }
	
	// Update is called once per frame
	void Update () {
        
        if(IsDD)
        {
            DDTimer -= Time.deltaTime;
            if(DDTimer < 0)
            {
                IsDD = false;
                DDTimer = 0;
            }
        }

        if (MyMobileInput.Tap)
        {
            ResetAll();
        }

        if (MyMobileInput.Touch)
        {
            if ((GetPositionFromScreen(MyMobileInput.StartPosition) - transform.position).magnitude < TouchSensitivity || TouchSensitivity == 0f)
                LoadChar = true;
        }
        
        if (LoadChar)
        {
            if (Load < MaxLoad)
                Load += Time.deltaTime * LoadMultiplier;
            else if (Load > MaxLoad)
                Load = MaxLoad;

            GetComponent<SpriteRenderer>().color = new Color(1 + Load, (MaxLoad - Load) / MaxLoad, (MaxLoad - Load) / MaxLoad);

        }

        if (MyMobileInput.Swipe && LoadChar)
        {
            Vector3 StartVector = GetPositionFromScreen(MyMobileInput.LastPosition)- transform.position;

            Quaternion SpawnQuat = Quaternion.FromToRotation(Vector3.up, StartVector);
            
            StartVector = StartVector.normalized * 2f + transform.position;

            Fire(StartVector, SpawnQuat);




            if (FireMultiplacator > 0)
            {
                int ForLength = FireMultiplacator > 2 ? FireMultiplacator * 4 - 4 : FireMultiplacator * 2;
                for (int i = 1; i < ForLength + 1; ++i)
                {
                    float SpawnAngleZ = Quaternion.FromToRotation(Vector3.up, StartVector - transform.position).eulerAngles.z;

                    SpawnAngleZ += MultiFireAngle * (i % 2 == 0 ? (-1f) : 1f) * ((i + 1) / 2);



                    if (SpawnAngleZ < 0)
                        SpawnAngleZ += 360;

                    print(SpawnAngleZ);

                    Vector3 VectorToSpawn = new Vector3();

                    VectorToSpawn.x = Mathf.Cos(Mathf.PI * (SpawnAngleZ + 90f) / 180f);
                    VectorToSpawn.y = Mathf.Sin(Mathf.PI * (SpawnAngleZ + 90f) / 180f);

                    Fire(VectorToSpawn * 2f + transform.position, Quaternion.Euler(0, 0, SpawnAngleZ));


                }
            }

            ResetAll();
        }

      
        if ((!MyMobileInput.IsDraging && LoadChar)|| MyMobileInput.Tap)
        {
            ResetAll();
        }
    }

    private Vector3 GetPositionFromScreen(Vector2 InScreenPos)
    {
        Vector3 MyVector = MyCamera.ScreenToWorldPoint(new Vector3(InScreenPos.x, InScreenPos.y, 0));
        MyVector.z = 0;

        return MyVector;
    }

    void ResetAll()
    {
        Load = 0f;
        LoadChar = false;

        GetComponent<SpriteRenderer>().color = Color.white;
    }

    public void MakeDD(float InTime)
    {
        IsDD = true;
        DDTimer = InTime;
    }

    private void Fire(Vector3 SpawnVector, Quaternion SpawnQuat)
    {
        GameObject MyBullet = null;

        if (BulletToShoot != null)
            MyBullet = Instantiate(BulletToShoot, SpawnVector, SpawnQuat);
        else
            print("Out of BulletToShoot (CharShooting)");

        if (MyBullet != null)
        {
            float LoadLvl = IsDD ? (1 + Load) * 2 : 1 + Load;
            MyBullet.GetComponent<Bullet>().SetLoadLevel(LoadLvl);
            if (IsDD)
                MyBullet.GetComponent<Bullet>().BulletSpeed *= 2;
            MyBullet.GetComponent<Bullet>().Tag = "Enemy";
            MyBullet.layer = 10;
        }
    }

    public void SetFireMultOnLevel(int InLevel)
    {
        switch (InLevel)
        {
            case -1:
                {
                    if (FireMultiplacator > 0)
                        --FireMultiplacator;
                    break;
                }
            case 5:
                {
                    if (FireMultiplacator < 5)
                        ++FireMultiplacator;
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
