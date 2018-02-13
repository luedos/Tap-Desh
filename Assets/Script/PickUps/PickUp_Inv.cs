using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp_Inv : PickUp
{
    public float InvinsibilityTime = 5f;         // How long Inv will last

    public override void PickUpMe(GameObject byObject)
    {
        PlayerHealth OtherHP = byObject.gameObject.GetComponent<PlayerHealth>();
        if (OtherHP != null)
        {
            GameManager.Instance.IncreaseGamePoints(PointsOnTake);
            OtherHP.MakeInvincible(InvinsibilityTime);
            Destroy(gameObject);
        }
    }

    // You know wtf here is going
    void OnTriggerEnter2D(Collider2D other)
    {
      
        if(other.tag == "Player")
        {
            PickUpMe(other.gameObject);
        }

    }
}
