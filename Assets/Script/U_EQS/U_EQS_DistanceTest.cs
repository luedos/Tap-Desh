using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ValueGraphic
{
    Defoult = 0,
    UpLinear = 1,
    DownLinear = 2
}

public class U_EQS_DistanceTest : U_EQS_Test {
    
    public float MinDistance = 0f;

    [Tooltip("If MaxValue less than MinValue, it will not count")]
    public float MaxDistance = 0f;

    [Tooltip("How value will change from Min to Max")]
    public ValueGraphic MyValueGraphic = ValueGraphic.Defoult;

    public override float CulcPoint(Vector3 InPoint) {

        if (TurnOffTest)
            return 1;

        float Dist = (MyQuery.GetPoint() - InPoint).magnitude;


        if (Dist < MinDistance)
            return MinMultiplier;

        if (MaxDistance > MinDistance && Dist > MaxDistance)
            return MinMultiplier;

        if(MyValueGraphic == 0)
        {
            return Multiplier;
        }

        float MyMax = MaxDistance > MinDistance ? MaxDistance : MyGenerator.GetMaxDistance();
        
        switch (MyValueGraphic)
        {
            case ValueGraphic.UpLinear:
                {
                    return MinMultiplier + (Multiplier - MinMultiplier) * (Dist - MinDistance) / (MyMax - MinDistance);
                }
            case ValueGraphic.DownLinear:
                {
                    return MinMultiplier + (Multiplier - MinMultiplier) * (MyMax - Dist) / (MyMax - MinDistance);
                }
            default:
                break;
        }

        return Multiplier;
    }

}
