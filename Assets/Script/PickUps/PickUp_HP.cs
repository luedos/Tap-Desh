using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp_HP : PickUp
{
    public int HealthToRegen = 3;               // How many hp bonus will regen


    public override void PickUpMe(GameObject byObject)
    {
        HealthPoints OtherHP = byObject.GetComponent<HealthPoints>();
        if (OtherHP != null)
        {
            GameManager.Instance.IncreaseGamePoints(PointsOnTake);
            OtherHP.RegenHP(HealthToRegen);
            Destroy(gameObject);
        }
    }


    // Set up health regen
    void OnTriggerEnter2D(Collider2D other)
    {

        if (other.tag == "Player")
        {
            PickUpMe(other.gameObject);
        }

    }
}
