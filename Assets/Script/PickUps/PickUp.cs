using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour {

    public int PointsOnTake = 3;                // How many points it will give on pickup

    private bool isLeft = true;

    public float DestroyInSec = 15f;

    private void Start()
    {
        if (DestroyInSec > 0)
            Destroy(gameObject, DestroyInSec);
    }

    void Update()
    {
        transform.position += Vector3.right * (isLeft ? -0.01f : 0.01f);
        isLeft = !isLeft;
    }

    public virtual void PickUpMe(GameObject byObject)
    {

    }
}
