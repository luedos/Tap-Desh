using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp_FM : MonoBehaviour {

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
            CharShooting MyShooting = other.GetComponent<CharShooting>();
            if (MyShooting != null)
            {
                MyShooting.SetFireMultOnLevel(5);
                Destroy(gameObject);
            }
        }

    }
}
