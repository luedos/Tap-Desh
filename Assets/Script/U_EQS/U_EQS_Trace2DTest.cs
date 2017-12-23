using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class U_EQS_Trace2DTest : U_EQS_Test {

    [Tooltip("if true test will give full multiplier in case when trace hit nothing")]
    public bool ForVisibility = true;
    
    [Tooltip("Needed only if query give not transform but position")]
    public string ForTag = "NONE";

    public override float CulcPoint(Vector3 InPoint) {

        if (TurnOffTest)
            return 1;

        Vector3 MyDestination = MyQuery.GetPoint();

        Vector3 MyDirection = MyDestination - InPoint;

        int MyVis = ForVisibility ? 1 : 0;

        int MyRay;
        RaycastHit2D MyRayCast = Physics2D.Raycast(InPoint, MyDirection, MyDirection.magnitude) ;

        if (!MyRayCast)
            MyRay = 1;
        else
            if (MyQuery.GetTransform() != null)
                MyRay = MyRayCast.transform == MyQuery.GetTransform() ? 1 : 0;
            else
                MyRay = MyRayCast.transform.tag == ForTag ? 1 : 0;
        
        

        if (MyRay - MyVis != 0)
            return MinMultiplier;
        else
            return Multiplier;
        
    }

}
