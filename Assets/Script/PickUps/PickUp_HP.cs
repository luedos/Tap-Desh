﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp_HP : MonoBehaviour
{
    public int HealthToRegen = 3;               // How many hp bonus will regen
    public int PointsOnTake = 3;                // How many points it will give on pickup

    private bool isLeft = true;


    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.right * (isLeft ? -0.01f : 0.01f);
        isLeft = !isLeft;
    }

    // Set up health regen
    void OnTriggerEnter2D(Collider2D other)
    {

        if (other.tag == "Player")
        {
            HealthPoints OtherHP = other.gameObject.GetComponent<HealthPoints>();
            if (OtherHP != null)
            {
                GameManager.Instance.IncreaseGamePoints(PointsOnTake);
                OtherHP.RegenHP(HealthToRegen);
                Destroy(gameObject);
            }
        }

    }
}