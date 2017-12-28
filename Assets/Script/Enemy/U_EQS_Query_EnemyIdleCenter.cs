using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class U_EQS_Query_EnemyIdleCenter : U_EQS_Query
{

    public override Vector3 GetPoint()
    {
        EnemyAI MyEnemy = GetComponent<EnemyAI>();
        if (MyEnemy != null)
                return MyEnemy.IdleCenter;

        return Vector3.zero;
    }

    public override Transform GetTransform()
    {
        return null;
    }
}
