using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp_BT : MonoBehaviour {


    public float BetterTPTime = 5f;

    private bool isLeft = true;

    void Update()
    {
        transform.position += Vector3.right * (isLeft ? -0.01f : 0.01f);
        isLeft = !isLeft;
    }


    void OnTriggerEnter2D(Collider2D other)
    {

        if (other.tag == "Player")
        {
            CharMovement OtherCM = other.gameObject.GetComponent<CharMovement>();
            if (OtherCM != null)
            {
                OtherCM.MakeBetterTP(BetterTPTime);
                Destroy(gameObject);
            }
        }

    }
}
