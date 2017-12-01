using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp_Inv : MonoBehaviour
{
    public float InvinsibilityTime = 5f;

    private bool isLeft = true;


    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.right * (isLeft ? -0.01f : 0.01f);
        isLeft = !isLeft;
    }


        void OnTriggerEnter2D(Collider2D other)
    {
        print("TriggerEnter");

        if(other.tag == "Player")
        {
            HealthPoints OtherHP = other.gameObject.GetComponent<HealthPoints>();
            if(OtherHP != null)
            {
                OtherHP.MakeInvincible(InvinsibilityTime);
                Destroy(gameObject);
            }
        }

    }
}
