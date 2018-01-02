using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

[System.Serializable]
public struct EnemySpawnersStruct
{
    public Spawner MySpawner;
    public int StartSpawnOnLvl;
}

public class GameManager : MonoBehaviour {

    private static GameManager instance = null;

    public static GameManager Instance
    { get { if (instance == null)
                instance = new GameObject().AddComponent<GameManager>();
            return instance;
          }
    }

    [HideInInspector]
    public GameObject Char = null;

    public Spawner[] BonusSpawners;
    public EnemySpawnersStruct[] EnemySpawners;

    public GameObject MainChar;
    public Transform BeginTransform;

    public GameObject myHUD;
    public GameObject myGameOverScreen;

    private int gamePoints = 0;
    private int Level = 0;


    public int GamePoints { get { return gamePoints; } }

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            if(instance != this)
            {
                Destroy(gameObject);
                return;
            }
        
    }

    public void IncreaseGamePoints(int InPoints)
    {
        gamePoints += InPoints;

        LevelUpdate();
    }

    void LevelUpdate()
    {
        int LocalLvl = GetLevelFromFormula(GamePoints);
        if (LocalLvl > Level)
        {
            SpawnBonus();

            for(int i = 0; i < EnemySpawners.Length; ++i)
            {
                if (EnemySpawners[i].StartSpawnOnLvl == LocalLvl)
                    EnemySpawners[i].MySpawner.StartSpawnTimer();
            }

            Level = LocalLvl;
        }
    }

    int GetLevelFromFormula(int InNumber)
    {
        int localInt;

        localInt = Mathf.FloorToInt(Mathf.Pow(0.2f * InNumber, 0.5f));

        return localInt;
    }

	// Use this for initialization
	void Start () {
        Char = GameObject.FindGameObjectWithTag("Player");
    }
	


    void SpawnBonus()
    {
        List<Spawner> LocalSpawn = new List<Spawner>(BonusSpawners);

        int LocalInt;
        int LocalCount = LocalSpawn.Count;

        for(int i = 0; i < LocalCount; ++i)
        {
            LocalInt = Random.Range(0, LocalSpawn.Count - 1);
            if (LocalSpawn[LocalInt].SpawnObjects())
                break;

            LocalSpawn.RemoveAt(LocalInt);
        }
    }

    public void GameOver()
    {
        GameStats.Instance.SetLastScore(GamePoints);

        myHUD.SetActive(false);
        myGameOverScreen.SetActive(true);
        
    }

    public void NewGame()
    {
        gamePoints = 0;
        Level = 0;

        GameObject[] Enemies = GameObject.FindGameObjectsWithTag("Enemy");

        int Num = Enemies.Length;
        
        for(int i = 0; i < Num; ++i)
        {
            if (Enemies[i].GetComponent<HealthPoints>() != null)
                Destroy(Enemies[i]);
        }

        GameObject[] MyBonuses = GameObject.FindGameObjectsWithTag("Bonus");

        for (int i = 0; i < MyBonuses.Length; ++i)
            Destroy(MyBonuses[i]);

        Spawner[] MySpawners = GameObject.FindObjectsOfType<Spawner>();

        int SpawnNum = MySpawners.Length;

        for (int i = 0; i < SpawnNum; ++i)
            MySpawners[i].RefreshSpawner();

        myHUD.GetComponent<HUDScript>().ResetBonuses();

        myHUD.SetActive(true);
        myGameOverScreen.SetActive(false);

        MainChar.GetComponent<HealthPoints>().MakeAllive();
        MainChar.transform.position = BeginTransform.position;


    }

    public void GoMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
