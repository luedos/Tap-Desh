using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp_BT : PickUp {


    public float BetterTPTime = 5f;         // How long BT will last

    public override void PickUpMe(GameObject byObject)
    {
        CharMovement OtherCM = byObject.GetComponent<CharMovement>();
        if (OtherCM != null)
        {
            OtherCM.MakeBetterTP(BetterTPTime);
            GameManager.Instance.IncreaseGamePoints(PointsOnTake);
            Destroy(gameObject);
        }
    }

    // Set up BT
    void OnTriggerEnter2D(Collider2D other)
    {

        if (other.tag == "Player")
        {
            PickUpMe(other.gameObject);
        }

    }
}
