using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp_BT : MonoBehaviour {


    public float BetterTPTime = 5f;         // How long BT will last
    public int PointsOnTake = 3;            // How many points it will give on pickup


    // this is only because unity collision bad
    private bool isLeft = true;
    void Update()
    {
        transform.position += Vector3.right * (isLeft ? -0.01f : 0.01f);
        isLeft = !isLeft;
    }

    // Set up BT
    void OnTriggerEnter2D(Collider2D other)
    {

        if (other.tag == "Player")
        {
            CharMovement OtherCM = other.gameObject.GetComponent<CharMovement>();
            if (OtherCM != null)
            {
                OtherCM.MakeBetterTP(BetterTPTime);
                GameManager.Instance.IncreaseGamePoints(PointsOnTake);
                Destroy(gameObject);
            }
        }

    }
}
