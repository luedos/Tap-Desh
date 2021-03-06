﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStats : MonoBehaviour {

    private static GameStats instance = null;

    public static bool bGamePaused = false;

    public static GameStats Instance
    {
        get
        {
            if (instance == null)
                instance = new GameObject().AddComponent<GameStats>();
            return instance;
        }
    }

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    public int LastScore = 0;
    public int HighScore = 0;

    public void SetLastScore(int inScore)
    {
        LastScore = inScore;
        if (LastScore > HighScore)
            HighScore = LastScore;
    }

    public static void PauseGame(bool isPause)
    {
        bGamePaused = isPause;

        if(isPause)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }
}
