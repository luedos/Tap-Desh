using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public struct LevelEvent
{
    public int InLevel;
    public UnityEvent InEvent;
}

[System.Serializable]
public enum EDifsType
{
    HP_Middle,
    HP_Fast,
    HP_Hard,
    Damage_Middle,
    Damage_Fast,
    Damage_Hard
}

[System.Serializable]
public struct Difs
{
    public EDifsType MyDif;
    public int StartLevel;
    public int LevelDelta;
    public int MaxLevel;

    [HideInInspector] public int MyLevel;

    public bool IsValid { get { return MyLevel < MaxLevel; } }
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
    public LevelEvent[] MyEvents;

    public GameObject MainChar;
    public Transform BeginTransform;

    public GameObject myHUD;
    public GameObject myGameOverScreen;

    [HideInInspector] public int plus_HP_Middle = 0;
    [HideInInspector] public int plus_HP_Fast = 0;
    [HideInInspector] public int plus_HP_Hard = 0;

    [HideInInspector] public int plus_Damage_Middle = 0;
    [HideInInspector] public int plus_Damage_Fast = 0;
    [HideInInspector] public int plus_Damage_Hard = 0;

    public Difs[] MyDifs;


    private float gamePoints = 0f;
    private float NextLvlPoints = 2f; 
    private int level = 0;

    public float GamePoints { get { return gamePoints; } }
    public int Level { get { return level; } }

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

    public void IncreaseGamePoints(float InPoints)
    {
        myHUD.GetComponent<HUDScript>().AddPoints(Mathf.FloorToInt(gamePoints + InPoints) - Mathf.FloorToInt(gamePoints));

        gamePoints += InPoints;
        
        if(gamePoints > NextLvlPoints)
            LevelUpdate();
    }

    void LevelUpdate()
    {
        NextLvlPoints = NextLvlPoints + Mathf.Pow(NextLvlPoints, 0.53f);

        ++level; 
        
        SpawnBonus();
        
        for(int i = 0; i < MyEvents.Length; ++i)
        {
            if (MyEvents[i].InLevel == level)
                MyEvents[i].InEvent.Invoke();
        }
        
        for(int i = 0; i < MyDifs.Length; ++i)
        {
            if(MyDifs[i].IsValid && MyDifs[i].MyLevel == level)
            {
                ExecuteDif(MyDifs[i].MyDif);
                MyDifs[i].MyLevel += MyDifs[i].LevelDelta;
        
            }
        }

    }

    // Use this for initialization
    void Start () {
        Char = GameObject.FindGameObjectWithTag("Player");

        NewGame();
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
        GameStats.Instance.SetLastScore(Mathf.FloorToInt(GamePoints));

        myHUD.SetActive(false);
        myGameOverScreen.SetActive(true);
        
    }

    public void ClearDificalty()
    {
        NextLvlPoints = 2f;

        level = 0;
        gamePoints = 0;

        for (int i = 0; i < MyEvents.Length; ++i)
        {
            if (MyEvents[i].InLevel == level)
                MyEvents[i].InEvent.Invoke();
        }

        plus_HP_Middle = 0;
        plus_HP_Fast = 0;
        plus_HP_Hard = 0;

        plus_Damage_Middle = 0;
        plus_Damage_Fast = 0;
        plus_Damage_Hard = 0;


        for(int i = 0; i < MyDifs.Length; ++i)
        {
            MyDifs[i].MyLevel = MyDifs[i].StartLevel;
        }

    }

    public void NewGame()
    {
        ClearDificalty();

        GameObject[] Enemies = GameObject.FindGameObjectsWithTag("Enemy");

        if (Enemies.Length > 0)
        {
            int Num = Enemies.Length;

            for (int i = 0; i < Num; ++i)
            {
                if (Enemies[i].GetComponent<HealthPoints>() != null)
                    Destroy(Enemies[i]);
            }
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

        CharPart[] MyCPs = MainChar.GetComponents<CharPart>();

        for(int i = 0; i < MyCPs.Length; ++i)
        {
            MyCPs[i].ResetPart();
        }

        MainChar.transform.position = BeginTransform.position;


    }

    public void GoMainMenu()
    {
       
        SceneManager.LoadScene(0);
    }

    void ExecuteDif(EDifsType InDif)
    {
        switch (InDif)
        {
            case EDifsType.HP_Middle:
                ++plus_HP_Middle;
                break;
            case EDifsType.HP_Fast:
                ++plus_HP_Fast;
                break;
            case EDifsType.HP_Hard:
                ++plus_HP_Hard;
                break;
            case EDifsType.Damage_Middle:
                ++plus_Damage_Middle;
                break;
            case EDifsType.Damage_Fast:
                ++plus_Damage_Fast;
                break;
            case EDifsType.Damage_Hard:
                ++plus_Damage_Hard;
                break;
            default:
                break;
        }
    }

}
