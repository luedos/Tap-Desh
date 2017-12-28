using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// all this script do, is return location of enemy where he was when he lost player 
public class U_EQS_Query_EnemyLastLoc : U_EQS_Query
{
    public override Vector3 GetPoint()
    {
        EnemyAI MyEnemy = GetComponent<EnemyAI>();
        if (MyEnemy != null)
                return MyEnemy.StartSerchLocation;

        return Vector3.zero;
    }

    public override Transform GetTransform()
    {
        return null;
    }

}
