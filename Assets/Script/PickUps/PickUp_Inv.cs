using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp_Inv : MonoBehaviour
{
    public float InvinsibilityTime = 5f;         // How long Inv will last
    public int PointsOnTake = 3;                 // How many points it will give on pickup

    private bool isLeft = true;

    public float DestroyInSec = 15f;

    private void Start()
    {
        if (DestroyInSec > 0)
            Destroy(gameObject, DestroyInSec);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.right * (isLeft ? -0.01f : 0.01f);
        isLeft = !isLeft;
    }

    // You know wtf here is going
        void OnTriggerEnter2D(Collider2D other)
    {
      
        if(other.tag == "Player")
        {
            HealthPoints OtherHP = other.gameObject.GetComponent<HealthPoints>();
            if(OtherHP != null)
            {
                GameManager.Instance.IncreaseGamePoints(PointsOnTake);
                OtherHP.MakeInvincible(InvinsibilityTime);
                Destroy(gameObject);
            }
        }

    }
}
