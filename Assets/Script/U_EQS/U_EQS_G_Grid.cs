using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class U_EQS_G_Grid : U_EQS_Generator
{
    [Tooltip("The number of dots from each side")]
    public int SideRange = 10;

    [Tooltip("Distance beetwen each dots")]
    public float DotDistanse = 0.5f;

    public override float GetMaxDistance()
    {
        return 0.70710678f * DotDistanse * SideRange;
    }

    public override bool FindSpot(out Vector3 InVector)
    {
        if (DrawPointOnFind)
            DrawPointOnFindTimer = DrawPointOnFindTime;

        Vector3 MyPos = CenterQuery.GetPoint();


        Vector2 StartPos = new Vector2();

        StartPos.x = MyPos.x - DotDistanse * (SideRange - 1) / 2f;
        StartPos.y = MyPos.y - DotDistanse * (SideRange - 1) / 2f;



        Points.Clear();



        int TestsLength = Tests.Length;

        for (int x = 0; x < SideRange; ++x)
        {
            
            LocalPoint.MyPoint.x = StartPos.x + x * DotDistanse;

            for (int y = 0; y < SideRange; ++y)
            {
               

                LocalPoint.MyPoint.y = StartPos.y + y * DotDistanse;
                LocalPoint.MyPriority = 1f;

                float LocalF = 1;

                for (int i = 0; i < TestsLength; ++i)
                {
                    if(Tests[i].MyTest == null)
                    {
                        print("Test #" + i + " fails " + transform.name);
                        break;
                    }

                    LocalF = Tests[i].MyTest.CulcPoint(LocalPoint.MyPoint);
                    if (LocalF == 0)
                        break;

                    LocalPoint.MyPriority *= LocalF;
                }

                if (LocalF == 0)
                    continue;

                if (Points.Count == 0) {
                    Points.Add(LocalPoint);
                    continue;
                }

                if(LocalPoint.MyPriority > Points[0].MyPriority)
                {
                    Points.Clear();
                    Points.Add(LocalPoint);
                    continue;
                }

                if(LocalPoint.MyPriority == Points[0].MyPriority)
                {
                    Points.Add(LocalPoint);
                }

            }
        }


      

        if(Points.Count == 0)
        {
            InVector = Vector3.zero;
            return false;
        }

        if(Points.Count > 1)
        {
            InVector = Points[Random.Range(0, Points.Count - 1)].MyPoint;
            LocalPoint.MyPoint = InVector;
            return true;
        }

        InVector = Points[0].MyPoint;
        LocalPoint.MyPoint = InVector;
        return true;
        
    }

    void OnDrawGizmosSelected()
    {
        if (DrawDebagPoints)
        {
            Vector3 MyPos = CenterQuery.GetPoint();

            Vector2 StartPos = new Vector2();

            StartPos.x = MyPos.x - DotDistanse * (SideRange - 1) / 2f;
            StartPos.y = MyPos.y - DotDistanse * (SideRange - 1) / 2f;

            

            for (int x = 0; x < SideRange; ++x)
            {
                MyPos.x = StartPos.x + x * DotDistanse;

                for (int y = 0; y < SideRange; ++y)
                {
                    MyPos.y = StartPos.y + y * DotDistanse;
                    Gizmos.DrawIcon(MyPos, "GeneratorPointIcon.png");
                }
            }
        }

        if(DrawPointOnFindTimer > 0f)
        {
            DrawPointOnFindTimer -= Time.deltaTime;
            if (Points.Count > 0)
                Gizmos.DrawIcon(LocalPoint.MyPoint, "FinalePointIcon.png");
            else
                Gizmos.DrawIcon(CenterQuery.GetPoint(), "RedCross.png");
        }
    }

}
