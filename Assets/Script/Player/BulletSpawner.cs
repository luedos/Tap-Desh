using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpawner : MonoBehaviour {

    public GameObject BulletToShoot;            // This thing buller will spawn
    public Transform ShootPoint;                // in which point buller will shoot (because it easier on editor)
    public float ShootDelay = 0f;               // Basicly rate of fire
    public float ShootTimeRange = 0f;           // Can use for some random in shoot delay
    public bool SpawnOnBegin = false;
    public string BulletTag = "Player";
    public int BulletLayer = 9;

    private float Timer = 0;
    private float NowMaxTimer;
    private bool isSpawn = false;
    

	// Use this for initialization
	void Start () {
        if (SpawnOnBegin)
            StartSpawn();
	}
	
	// push timer forward..
	void Update () {
		if(isSpawn)
        {
            Timer += Time.deltaTime;
            // and when timer reach end..
            if(Timer >= NowMaxTimer)
            {
                // we spawn bullet and reload timer
                SpawnBullet();
                StartSpawn();
            }
        }
	}

    // reload timer
    public void StartSpawn()
    {
        isSpawn = true;
        Timer = 0f;
        NowMaxTimer = ShootDelay + Random.Range(-ShootTimeRange, ShootTimeRange);
    }

    // stop timer
    public void EndSpawn() { isSpawn = false; }


    // Simply spawn bullet
    public void SpawnBullet()
    {
        GameObject MyBullet = null;

        if (BulletToShoot != null)
            MyBullet = Instantiate(BulletToShoot, ShootPoint.position, Quaternion.FromToRotation(Vector3.up, ShootPoint.position - transform.position));
        else
            print("Out of BulletToShoot : " + gameObject.name);

        if (MyBullet != null)
        {
            MyBullet.GetComponent<Bullet>().Tag = BulletTag;
            MyBullet.layer = BulletLayer;
        }
    }
}
