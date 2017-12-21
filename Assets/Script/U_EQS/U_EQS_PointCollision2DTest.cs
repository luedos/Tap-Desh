using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class U_EQS_PointCollision2DTest : U_EQS_Test {

    [Tooltip("if true test will give full multiplier in case when collision give true")]
    public bool IsColliding = true;

    public override float CulcPoint(Vector3 InPoint)
    {
        if (TurnOffTest)
            return 1;

        int MyColl = Physics2D.OverlapPoint(InPoint) ? 1 : 0;
        int MyVis = IsColliding ? 1 : 0;

        if (MyColl - MyVis != 0)
            return MinMultiplier;
        else
            return Multiplier;
    }

}
