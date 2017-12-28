using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp_FM : MonoBehaviour {

    public int PointsOnTake = 3;           // How many points it will give on pickup


    // I am tired from unity collision
    private bool isLeft = true;
    void Update()
    {
        transform.position += Vector3.right * (isLeft ? -0.01f : 0.01f);
        isLeft = !isLeft;
    }

    // Set up fire mode
    void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.tag == "Player")
        {
            CharShooting MyShooting = other.GetComponent<CharShooting>();
            if (MyShooting != null)
            {
                GameManager.Instance.IncreaseGamePoints(PointsOnTake);
                MyShooting.SetFireMultOnLevel(5);
                Destroy(gameObject);
            }
        }

    }
}
