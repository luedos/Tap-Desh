using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct U_EQS_Point
{
    public Vector3 MyPoint;
    public float MyPriority;
}
[System.Serializable]
public class TestArrayItem
{
    public string Name = "MyTest";

    public U_EQS_Test MyTest;
}

public class U_EQS_Generator : MonoBehaviour {

    public TestArrayItem[] Tests;

    [Tooltip("By that let know what is center")]
    public U_EQS_Query CenterQuery;

    public bool DrawDebagPoints = false;
    public bool DrawPointOnFind = false;
    public float DrawPointOnFindTime = 2f;

    protected float DrawPointOnFindTimer = 0f;

    protected U_EQS_Point LocalPoint = new U_EQS_Point();

    protected List<U_EQS_Point> Points = new List<U_EQS_Point>();

    public void Start()
    {
        for (int i = 0; i < Tests.Length; ++i)
            Tests[i].MyTest.MyGenerator = this;
    }

    public virtual bool FindSpot(out Vector3 InVector)
    {
        
        InVector = Vector3.zero;
        return false;
    }

    public virtual float GetMaxDistance() { return 0f; }
}
