using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugText : MonoBehaviour {

    public Text MyText;
    public float LightTime = 2f;
    private float Timer = 0f;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Timer > 0)
        {
            Timer -= Time.deltaTime;
            Color LocalColor = MyText.color;
            LocalColor.a = Timer / LightTime;
            MyText.color = LocalColor;
        }
	}

    public void TurnOnText()
    {
        Timer = LightTime;
    }

    public void TurnOnText(string InText)
    {
        MyText.text = InText;
        Timer = LightTime;
    }

}
