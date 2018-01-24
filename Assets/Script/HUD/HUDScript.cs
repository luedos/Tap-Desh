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

    public Text ValueText;
    public float MinVal;
    public float MaxVal;
    public GameObject Options;

    public BonusIcon InvIcon;
    public BonusIcon BTIcon;
    public BonusIcon DDIcon;
    public BonusIcon_ByNumber FMIcon;

    public float ScreenSize = 200;


    // Use this for initialization
    void Start () {

        if (!FindChar())
            print("No char was found : " + name);

        

        float currentAspect = (float)Screen.width / (float)Screen.height;
        Camera.main.orthographicSize = Screen.currentResolution.width / currentAspect / ScreenSize;

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

    public void AddBonusIcon_Inv(float ByTime)
    {
        InvIcon.MyTimer = ByTime;
    }

    public void AddBonusIcon_DD(float ByTime)
    {
        DDIcon.MyTimer = ByTime;
    }
    public void AddBonusIcon_FM(int InIncreaser)
    {
        
        FMIcon.IncreaseNumber(InIncreaser);
    }
    public void AddBonusIcon_BT(float ByTime)
    {
        BTIcon.MyTimer = ByTime;
    }

    public void ResetBonuses()
    {
        InvIcon.MyTimer = 0f;
        DDIcon.MyTimer = 0f;
        BTIcon.MyTimer = 0f;
        FMIcon.MyNumber = 0;
    }

    public void OnValueUpdated(float InVal)
    {
        if (MyChar == null)
            if (!FindChar())
                return;
        float val = MinVal + (MaxVal - MinVal) * InVal;
        
        MyChar.GetComponent<MobileInput>().TapTimeSensitivity = val;
        ValueText.text = val.ToString();
    }

    public void OpenCloseOptions()
    {
        Options.SetActive(!Options.activeSelf);
    }
}
