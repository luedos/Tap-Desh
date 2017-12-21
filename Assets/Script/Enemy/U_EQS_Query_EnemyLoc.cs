using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class U_EQS_Query_EnemyLoc : U_EQS_Query{

    public override Vector3 GetPoint()
    {
        EnemyAI MyEnemy = GetComponent<EnemyAI>();
        if (MyEnemy != null)
            if (MyEnemy.Enemy != null)
                return MyEnemy.Enemy.transform.position;
               
        return Vector3.zero;
        
    }

    public override Transform GetTransform()
    {
        EnemyAI MyEnemy = GetComponent<EnemyAI>();
        if (MyEnemy != null)
            if (MyEnemy.Enemy != null)
                return MyEnemy.Enemy.transform;

        return null;
    }
}
