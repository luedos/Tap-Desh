using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharShooting : MonoBehaviour {


    public GameObject BulletToShoot;
    public float MaxLoad = 2;
    public float TouchSensitivity = 0f;
    public float LoadMultiplier = 1f;

    private Camera MyCamera;
    private MobileInput MyMobileInput;
    private bool LoadChar = false;
    private float Load = 0f;
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
            Vector3 SpawnVector = GetPositionFromScreen(MyMobileInput.LastPosition)- transform.position;

            Quaternion SpawnQuat = Quaternion.FromToRotation(Vector3.up, SpawnVector);
            
            SpawnVector = (SpawnVector/SpawnVector.magnitude) * 2f + transform.position;

            GameObject MyBullet = null;

            if (BulletToShoot != null)
                MyBullet = Instantiate(BulletToShoot, SpawnVector, SpawnQuat);
            else
                print("Out of BulletToShoot (CharShooting)");

            if (MyBullet != null)
            {
                MyBullet.GetComponent<Bullet>().SetLoadLevel(1 + Load);
                MyBullet.GetComponent<Bullet>().Tag = "Enemy";
                MyBullet.layer = 10;
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
}
