using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDScript : MonoBehaviour {

    public GameObject PointerCenter = null;

    public Canvas MyCanvas = null;
    public GameObject MyHeart = null;
    public Text PointsText = null;
    GameObject MyChar = null;
	// Use this for initialization
	void Start () {

        if (!FindChar())
            print("No char was found : " + name);


        

        UpdateHealths();
	}
	
	// Update is called once per frame
    void Update()
    {
        PointsText.text = GameManager.Instance.GamePoints.ToString();
    }
	bool FindChar()
    {
       MyChar = GameManager.Instance.Char;
        if (MyChar == null)
        {
            MyChar = GameObject.FindGameObjectWithTag("Player");
            if (MyChar == null)
            {
                return false;
            }
        }

        MyChar.GetComponent<HealthPoints>().DamageTaken.Add(new HealthPoints.SimpleDelegate(UpdateHealths));

        return true;
    }

    void UpdateHealths()
    {
        if (MyHeart != null && MyCanvas != null)
        {
            

            if (MyChar == null)
                if (!FindChar())
                    return;

            for (int i = 0; i < MyCanvas.transform.childCount; ++i)
                Destroy(MyCanvas.transform.GetChild(i).gameObject);

            int num = MyChar.GetComponent<HealthPoints>().HP;
            for (int i = 0; i < num; i++)
            {
                GameObject LocalHeart = Instantiate(MyHeart);
                LocalHeart.transform.SetParent(MyCanvas.transform);
            }
        }
    }
}
