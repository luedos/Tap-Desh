using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : HealthPoints {

    [Tooltip("Needed only for enemy")]
    public int PointsForDeath = 13;                 // how many points we will recive on enemy killing

    public bool DestroyAfterDeath = true;           // When owner is die, want you to destroy it, or just make invisible without collision
    [Tooltip("How many time will pass after death, before owner will be destroy (only if DestroyAfterDeath = true)")]
    public float TimeBeforDestroy = 1f;

    protected override void Death()
    {
        GameManager.Instance.IncreaseGamePoints(PointsForDeath);


        if (DestroyAfterDeath)
            Destroy(gameObject, TimeBeforDestroy);

        base.Death();
    }
}
