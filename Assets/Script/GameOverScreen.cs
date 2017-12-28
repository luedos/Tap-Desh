using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class GameOverScreen : MonoBehaviour {

    public Text MaxScoreText;
    public Text ScoreText;
	


    void OnEnable()
    {
        MaxScoreText.text = GameStats.Instance.HighScore.ToString();
        ScoreText.text = GameStats.Instance.LastScore.ToString();
    }
}
