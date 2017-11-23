using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPoints : MonoBehaviour {

    public float HP = 100f;

    public void DoDamage(float Damage)
    {
        

        HP -= Damage;

        if (HP <= 0f)
        {
            
            Destroy(gameObject);
        }
    }
}
