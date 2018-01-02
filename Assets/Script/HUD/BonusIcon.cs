using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class BonusIcon : MonoBehaviour {

    public string MyName;

    [HideInInspector]
    public float MyTimer = 0f;

    

    public Image MyImage;


	// Use this for initialization
	void Start () {

    }
	
	// Update is called once per frame
	void Update () {

        if (MyTimer > 0f)
            MyTimer -= Time.deltaTime;
        


        if (MyImage.enabled)
        {
            if (MyTimer <= 0f)
            {                
                MyImage.enabled = false;
            }
        }
        else
            if (MyTimer > 0)
                MyImage.enabled = true;

	}
}
