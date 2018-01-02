using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ChanceObject
{
    public GameObject MyObject;
    public float MyChance;
}

public class Spawner : MonoBehaviour {

    public ChanceObject[] MyChanceObjects;

    [Tooltip("If 0, timer will not count")]
    public float SpawnTimer = 0f;

    [Tooltip("If empty will spawn one object on spawner transform")]
    public Transform[] SpawnTransforms;
        
    public int NumberOfSpawn = 1;

    [Tooltip("Before spawn it checks is in this radius exsist collider, if yes it will not spawn, if 0 it will not check")]
    public float SpawnRadius = 1;

    public bool SpawnOnCreate = false;

    private float Timer = 0f;
    private bool bSpawnByTimer = false;

    // Use this for initialization
    void Start()
    {
        if (SpawnTimer > 0f)
        {
            if (SpawnOnCreate)
                bSpawnByTimer = true;
        }
        else
            if (SpawnOnCreate)
            SpawnObjects();
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

        float MaxChance = 0f;

        for (int i = 0; i < MyChanceObjects.Length; ++i)
            MaxChance += MyChanceObjects[i].MyChance;

        float MyChance = Random.Range(0f, MaxChance);
        MaxChance = 0f;

        for(int i = 0; i < MyChanceObjects.Length; ++i)
        {
            MaxChance += MyChanceObjects[i].MyChance;
            if(MaxChance > MyChance)
            {
                OutObject = MyChanceObjects[i].MyObject;
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
}
