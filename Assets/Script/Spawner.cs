using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ChanceObject
{
    public GameObject MyObject;
    public float MyChance;
}

public class SpawnListObject
{
    public SpawnListObject(int inIndex, int inSpawnNumber)
    {
        MyIndex = inIndex;
        MySpawnNumber = inSpawnNumber;
    }

    public int MyIndex;
    public int MySpawnNumber;
}


public class Spawner : MonoBehaviour {

    public ChanceObject[] MyChanceObjects;

    [Tooltip("If 0, timer will not count")]
    public float SpawnTimer = 0f;

    [Tooltip("If empty will spawn one object on spawner transform")]
    public Transform[] SpawnTransforms;

    public int maxSpawnNumber = 0;
    public int minSpawnNumber = 0;

    public int MaxSpawnNumber { get { return maxSpawnNumber; } set { maxSpawnNumber = value; } }
    public int MinSpawnNumber { get { return minSpawnNumber; } set { minSpawnNumber = value; } }

    public int minLimit = 0;
    public int maxLimit = 0;
    public string Tag;

    public int MaxLimit { get { return maxLimit; } set { maxLimit = value; } }
    public int MinLimit { get { return minLimit; } set { minLimit = value; } }

    private int NumberOfSpawn = 1;

    [Tooltip("Before spawn it checks is in this radius exsist collider, if yes it will not spawn, if 0 it will not check")]
    public float SpawnRadius = 1;

    public bool SpawnOnCreate = false;

    private float Timer = 0f;
    private bool bSpawnByTimer = false;

    private List<SpawnListObject> SpawnList = new List<SpawnListObject>();

    // Use this for initialization
    void Start()
    {

        ResetChances();

        if (SpawnTimer > 0f)
        {
            if (SpawnOnCreate)
            {
                bSpawnByTimer = true;
                SpawnObjects();
            }
        }
        else
            if (SpawnOnCreate)
            SpawnObjects();



    }

    public bool ResetChances()
    {
        if (MyChanceObjects.Length == 0)
            return false;

        SpawnList.Clear();

        if(MyChanceObjects.Length == 1)
        {
            SpawnList.Add(new SpawnListObject(0, 100));
            return true;
        }

        float MinChanse = MyChanceObjects[0].MyChance;

        for(int i = 1; i < MyChanceObjects.Length; ++i)
            if (MyChanceObjects[i].MyChance < MinChanse)
                MinChanse = MyChanceObjects[i].MyChance;
        

        MinChanse = Mathf.Ceil(MinChanse) / MinChanse;

        for(int i = 0; i < MyChanceObjects.Length; ++i)
        {
            if(MyChanceObjects[i].MyChance > 0f)
                SpawnList.Add(new SpawnListObject(i, Mathf.CeilToInt(MyChanceObjects[i].MyChance * MinChanse)));
        }

        
        return SpawnList.Count > 0;
    }

    bool ChooseObject(out GameObject OutObject)
    {
        if (MyChanceObjects.Length == 0)
        {
            OutObject = null;
            return false;
        }

        if(MyChanceObjects.Length == 1)
        {
            OutObject = MyChanceObjects[0].MyObject;
            return true;
        }

        if (SpawnList.Count == 0)
           if( !ResetChances())
            {
                OutObject = null;
                return false;
            }


        float MaxChance = 0f;

        for (int i = 0; i < SpawnList.Count; ++i)
            MaxChance += SpawnList[i].MySpawnNumber;
        

        float MyChance = Random.Range(0, MaxChance);

        MaxChance = 0;

        for(int i = 0; i < SpawnList.Count; ++i)
        {
            MaxChance += SpawnList[i].MySpawnNumber;
            if(MaxChance > MyChance)
            {
                OutObject = MyChanceObjects[SpawnList[i].MyIndex].MyObject;
                --SpawnList[i].MySpawnNumber;
                if (SpawnList[i].MySpawnNumber < 1)
                    SpawnList.RemoveAt(i);
                return true;
            }
        }

        OutObject = null;
        return false;
    }

    public bool SpawnObjects()
    {
        if (MyChanceObjects.Length == 0)
            return false;

        if (minSpawnNumber >= maxSpawnNumber)
            NumberOfSpawn = minSpawnNumber;
        else
            NumberOfSpawn = Random.Range(minSpawnNumber, maxSpawnNumber + 1);

        if (maxLimit > 0)
        {
            int Enemies = GameObject.FindGameObjectsWithTag(Tag).Length;
            if (Enemies >= maxLimit)
                return true;

            if (Enemies > minLimit && maxLimit > minLimit)
                NumberOfSpawn = Mathf.CeilToInt(NumberOfSpawn * ((float)(maxLimit - Enemies)) / (maxLimit - minLimit));
        }


        if (SpawnTransforms.Length == 0)
        {            
            return SpawnOnPosition(transform.position);
        }

        bool WhatReturn = false;

        if (SpawnTransforms.Length <= NumberOfSpawn)
        {
            
            for (int i = 0; i < SpawnTransforms.Length; ++i)
                if (SpawnOnPosition(SpawnTransforms[i].position))
                    WhatReturn = true;

            return WhatReturn;
        }

        List<Transform> LocalTransforms = new List<Transform>(SpawnTransforms);

        int LocalInt;
        
        for(int i = 0; i < NumberOfSpawn; ++i)
        {
            if (LocalTransforms.Count == 0)
                break;

            LocalInt = Random.Range(0, LocalTransforms.Count - 1);
            if (!SpawnOnPosition(LocalTransforms[LocalInt].position))
                --i;
            else
                WhatReturn = true;

            LocalTransforms.RemoveAt(LocalInt);
        }

        return WhatReturn;
    }


    bool SpawnOnPosition(Vector3 InPos)
    {
        GameObject LocalObject;

        if (!ChooseObject(out LocalObject))
            return false;

        if(SpawnRadius == 0f)
        {
            Instantiate(LocalObject, InPos, Quaternion.identity);
            return true;
        }


        if (Physics2D.OverlapCircle(InPos, SpawnRadius))
            return false;
        else
            Instantiate(LocalObject, InPos, Quaternion.identity);

        return true;
    }

    public void StopSpawnTimer()
    {
        bSpawnByTimer = false;
    }

    public void StartSpawnTimer()
    {
        bSpawnByTimer = true;
    }

	// Update is called once per frame
	void Update () {
		
        if(bSpawnByTimer && SpawnTimer > 0f)
        {
            Timer -= Time.deltaTime;

            if(Timer < 0)
            {
                SpawnObjects();
                Timer = SpawnTimer;
            }
        }
        

	}

    public void RefreshSpawner()
    {
        Timer = 0;

        if (!SpawnOnCreate && bSpawnByTimer)
            bSpawnByTimer = false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="SpawnObjectIndex"></param>
    /// <param name="inPercentage"> Chence </param>
    public void SetSpawnPercentage(int SpawnObjectIndex, float Chance)
    {
        if(SpawnObjectIndex < MyChanceObjects.Length)
        {
            MyChanceObjects[SpawnObjectIndex].MyChance = Chance;
            ResetChances();
        }
    }

    /// <summary>
    /// Change Chance for object by Index
    /// </summary>
    /// <param name="inLine"> format of string : Index|Percentage 
    /// <para> Index - int param, show on which index in array percentage will change </para>
    /// <para> Percentage - float param, Chance of object  </para> </param>
    public void SetSpawnPercentage(string inLine)
    {

        string[] MySs = inLine.Split('|');

        if (MySs.Length != 2)
            return;

        int Index;
        if (!int.TryParse(MySs[0], out Index))
            return;

        if (Index >= MyChanceObjects.Length)
            return;

        float Chance;
        if (!float.TryParse(MySs[1], out Chance))
            return;

        MyChanceObjects[Index].MyChance = Chance;

        

        ResetChances();        
    }

}
