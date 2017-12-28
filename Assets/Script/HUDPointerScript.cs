using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDPointerScript : MonoBehaviour {

    [HideInInspector]
    public GameObject Enemy = null;
    [HideInInspector]
    public GameObject Char = null;

    public Image MyImage;
    public float MaxDistance = 40f;
    public float MinDistance = 10f;
    public float MaxAlpha = 0.7f;
    public float MinAlpha = 0.1f;
    public Color RegularColor = Color.red;
	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {

        

        if (Enemy == null)
        {
            
            Destroy(gameObject);
            return;
        }

        float Dist = (Enemy.transform.position - Char.transform.position).magnitude;

        if(Dist > MaxDistance)
        {
            MyImage.color = Color.clear;
            return;
        }

        if (Dist < MinDistance)
            Dist = MinDistance;

        float EndAlpha = MinAlpha + MaxAlpha * (MaxDistance - Dist) / (MaxDistance - MinDistance);

        RegularColor.a = EndAlpha;

        MyImage.color = RegularColor;

        transform.rotation = Quaternion.FromToRotation(Vector2.up, (Vector2)(Enemy.transform.position - Char.transform.position));
	}
}
