using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflectBullets : MonoBehaviour {



    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Bullet")
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
  
    void OnTriggerEnter2D(Collider2D other)
    {
       
  
        if (other.gameObject.tag == "Bullet")
        {
            
            ContactPoint2D[] MyContactPoint = new ContactPoint2D[10];
  
  
  
          if (other.GetContacts(MyContactPoint) > 0)
          {
              
  
                float NextAngle = other.transform.rotation.z + 180 - 2 * Quaternion.FromToRotation(Vector3.up, MyContactPoint[0].normal).z;
              
                other.transform.Rotate(0, 0, NextAngle);
                Rigidbody2D MyRig = other.gameObject.GetComponent<Rigidbody2D>();
                if (MyRig != null)
                {
                    
                    MyRig.velocity = MyRig.transform.up * MyRig.velocity.magnitude;
                }
            }
        }
    }
}
