using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpawner : MonoBehaviour {

    public GameObject BulletToShoot;
    public Transform ShootPoint;
    public float ShootDelay = 0f;
    public float ShootTimeRange = 0f;
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
	
	// Update is called once per frame
	void Update () {
		if(isSpawn)
        {
            Timer += Time.deltaTime;
            if(Timer >= NowMaxTimer)
            {
                SpawnBullet();
                StartSpawn();
            }
        }
	}

    public void StartSpawn()
    {
        isSpawn = true;
        Timer = 0f;
        NowMaxTimer = ShootDelay + Random.Range(-ShootTimeRange, ShootTimeRange);
    }

    public void EndSpawn() { isSpawn = false; }

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
