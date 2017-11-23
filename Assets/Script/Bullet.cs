using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    public float Damage = 35;
    public float BulletSpeed = 10f;
    public float LifeTime = 5f;

    private string MyTag;
    private float LoadLevel = 1;

    public string Tag { get { return MyTag; } set { MyTag = value; } }

	// Use this for initialization
	void Start () {
        GetComponent<Rigidbody2D>().velocity = transform.up * BulletSpeed;
        Destroy(gameObject, LifeTime);
    }
	
	// Update is called once per frame
	void Update () {
       // LocalLifeTime += Time.deltaTime;
       //
       // if (LocalLifeTime > LifeTime)
            
	}

    public void BulletDestroy()
    {



        Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == MyTag)
        {
            
            HealthPoints EnemyHealth = other.gameObject.GetComponent<HealthPoints>();
            if (EnemyHealth != null)
            {
                EnemyHealth.DoDamage(Damage * LoadLevel);
                
            }

            
            BulletDestroy();

        }
        else
            if (other.gameObject.tag != "NoBullet")
               BulletDestroy();
            
    }   

    

    public void SetLoadLevel(float LoadLvl)
    { 
        if (LoadLvl < 1)
            LoadLevel = 1;
        else
            LoadLevel = Mathf.Floor(LoadLvl);

        const float MaxLoad = 3;

        GetComponent<SpriteRenderer>().color = new Color(1 + LoadLevel, (MaxLoad - LoadLevel) / MaxLoad, (MaxLoad - LoadLevel) / MaxLoad);

    }
}
