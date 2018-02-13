using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp_DD : PickUp {

    public float DDTime = 5;                 // How long DD will last

    public override void PickUpMe(GameObject byObject)
    {
        CharShooting MyShooting = byObject.GetComponent<CharShooting>();
        if (MyShooting != null)
        {
            GameManager.Instance.IncreaseGamePoints(PointsOnTake);
            MyShooting.MakeDD(DDTime);
            Destroy(gameObject);
        }
    }

    // Set up DD
    void OnTriggerEnter2D(Collider2D other)
    {      
        if (other.tag == "Player")
        {
            PickUpMe(other.gameObject);
        }
    }
}
