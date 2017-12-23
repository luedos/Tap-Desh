using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPoints : MonoBehaviour {

    public int HP = 3;
    public bool IsInvincible { get { return isInvincible; } }


    private float InvinsibilityTimer = 0f;
    private bool isInvincible = false;

    public void DoDamage(int Damage)
    {
        
        if(!isInvincible)
            HP -= Damage;

        if (HP <= 0f)
        {
            
            Destroy(gameObject);
        }
    }

    void Update()
    {
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

    public void MakeInvincible(float OnTime)
    {
        InvinsibilityTimer = OnTime;
        isInvincible = true;
    }

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
}
