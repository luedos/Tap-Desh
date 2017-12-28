using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPoints : MonoBehaviour {

    public int MaxHP = 3;                                                   // so we can't overheal ourself
    public bool IsInvincible { get { return isInvincible; } }

    // all of this so some scripts could subscribe and know if our hp is changed
    public delegate void SimpleDelegate();
    public List<SimpleDelegate> DamageTaken = new List<SimpleDelegate>();

    [Tooltip("Needed only for enemy")]
    public int PointsForDeath = 13;                 // how many points we will recive on enemy killing

    public bool DestroyAfterDeath = true;           // When owner is die, want you to destroy it, or just make invisible without collision
    [Tooltip("How many time will pass after death, before owner will be destroy (only if DestroyAfterDeath = true)")]
    public float TimeBeforDestroy = 1f;


    private bool isDead = false;
    private float InvinsibilityTimer = 0f;
    private bool isInvincible = false;
    private int hp = 0;
    private int startLayer;

    public int HP { get { return hp; } }
    public bool IsDead { get { return isDead; } }

    // call evere method which subscribe on it (also if can't call some method we deleting them)
    void CallHPSubs()
    {
        for (int i = 0; i < DamageTaken.Count; ++i)
        {
            if (DamageTaken[i] == null)
            {
                DamageTaken.RemoveAt(i);
                --i;
                continue;
            }
            DamageTaken[i]();
        }
    }

    // seting our actual HP
    void Start()
    {
        hp = MaxHP;
        startLayer = gameObject.layer;
    }

    // on damage taken
    public void DoDamage(int Damage)
    {
        // if we are not invincible we recive damage
        if(!isInvincible)
            hp -= Damage;


        CallHPSubs();


        // and if our health is less then we can survive we destroing ourself
        if (hp <= 0f)
        {
            isDead = true;

            if (tag == "Enemy")
                GameManager.Instance.IncreaseGamePoints(PointsForDeath);


            GetComponent<SpriteRenderer>().enabled = false;
            gameObject.layer = 16;                                  // IgnoreAll layer

            if(DestroyAfterDeath)
                Destroy(gameObject,TimeBeforDestroy);

            if (tag == "Player")
                GameManager.Instance.GameOver();

            MobileInput MyMI = GetComponent<MobileInput>();
            if (MyMI != null)
                MyMI.BlockInput = true;
        }


    }

    // siply regen hp
    public void RegenHP(int InHP)
    {
        // if there is more then we need we set it as max
        hp += InHP;
        if (hp > MaxHP)
            hp = MaxHP;

        CallHPSubs();

    }

    void Update()
    {
        // just move invincibility timer
        if (InvinsibilityTimer > 0)
        {
            InvinsibilityTimer -= Time.deltaTime;

            if(InvinsibilityTimer <=0)
            {
                isInvincible = false;
                InvinsibilityTimer = 0;
            }
        }

        
    }
    // set up invincibility
    public void MakeInvincible(float OnTime)
    {
        InvinsibilityTimer = OnTime;
        isInvincible = true;
    }

    // basicly if we hit bullet but we are invinsible we will reflect it
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Bullet" && isInvincible)
        {
            ContactPoint2D[] MyContactPoint = new ContactPoint2D[10];
            
            if (other.GetContacts(MyContactPoint) > 0)
            {
                Vector3 TestAngle;

                TestAngle = 2 * Quaternion.FromToRotation(Vector3.up, MyContactPoint[0].normal).eulerAngles - other.transform.rotation.eulerAngles;

                TestAngle.z -= 180;

                other.transform.eulerAngles = TestAngle;

                Rigidbody2D MyRig = other.gameObject.GetComponent<Rigidbody2D>();
                Bullet MyBull = other.gameObject.GetComponent<Bullet>();
                if (MyRig != null && MyBull != null)
                {
                    MyRig.velocity = MyRig.transform.up * MyBull.BulletSpeed;
                }
            }
        }
    }

    public void MakeAllive()
    {
        if(isDead)
        {
            GetComponent<SpriteRenderer>().enabled = true;
            gameObject.layer = startLayer;
            
            MakeInvincible(3f);

            hp = MaxHP;

            CallHPSubs();

            MobileInput MyMI = GetComponent<MobileInput>();
            if (MyMI != null)
                MyMI.BlockInput = false;

            isDead = false;
        }
    }
}
