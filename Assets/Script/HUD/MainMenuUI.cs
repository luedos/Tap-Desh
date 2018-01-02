using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour {

    public Text ScoreText;
    public Text HighScoreText;

    void Start()
    {
        ScoreText.text = GameStats.Instance.LastScore.ToString();
        HighScoreText.text = GameStats.Instance.HighScore.ToString();
    }

    public void GoPlay()
    {
        SceneManager.LoadScene(1);
    }

    public void GoQuit()
    {
        Application.Quit();
    }
}
