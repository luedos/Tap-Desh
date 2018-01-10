using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPoints : MonoBehaviour {

    public int MaxHP = 3;                                                   // so we can't overheal ourself
    public bool IsInvincible { get { return isInvincible; } }

    // all of this so some scripts could subscribe and know if our hp is changed
    public delegate void SimpleDelegate();
    public List<SimpleDelegate> DamageTaken = new List<SimpleDelegate>();


    private bool isDead = false;
    protected float InvinsibilityTimer = 0f;
    protected bool isInvincible = false;
    private int hp = 0;
    private int startLayer;

    public int HP { get { return hp; } }
    public bool IsDead { get { return isDead; } }

    // call evere method which subscribe on it (also if can't call some method we deleting them)
    protected void CallHPSubs()
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
    public virtual void Start()
    {
        hp = MaxHP;
        startLayer = gameObject.layer;
    }

    // on damage taken
    public void DoDamage(int Damage)
    {
        
        if(!isInvincible)
            hp -= Damage;

        CallHPSubs();
        
        if (hp <= 0f)
            Death();

    }

    // siply regen hp
    public void RegenHP(int InHP)
    {
        
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

    protected virtual void Death()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        gameObject.layer = 16;                                  // IgnoreAll layer

        isDead = true;
    }
    // set up invincibility
    public void MakeInvincible(float OnTime)
    {
        if(OnTime == 0f)
        {
            if(isInvincible)
            {
                InvinsibilityTimer = 0f;
                isInvincible = false;
                GameManager.Instance.myHUD.GetComponent<HUDScript>().AddBonusIcon_Inv(0f);
            }

            return;
        }

        InvinsibilityTimer = OnTime;
        isInvincible = true;
        GameManager.Instance.myHUD.GetComponent<HUDScript>().AddBonusIcon_Inv(OnTime);
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

    // Return all variables on defoult from death state
    public virtual void MakeAllive()
    {
        if(isDead)
        {
            GetComponent<SpriteRenderer>().enabled = true;
            gameObject.layer = startLayer;
            
            MakeInvincible(3f);

            hp = MaxHP;

            CallHPSubs();

            isDead = false;
        }
    }
}
