using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BonusIcon_ByNumber : BonusIcon {

    [HideInInspector]
    public int MyNumber = 0;
    public Text MyNumberText;

    void Start()
    {
        MyNumberText.text = MyNumber.ToString();
    }

    void Update()
    {
        if (MyImage.enabled)
        {
            if (MyNumber == 0)
            {
                MyImage.enabled = false;
                MyNumberText.enabled = false;
            }
        }
        else
            if (MyNumber > 0)
            {
                MyImage.enabled = true;
                MyNumberText.enabled = true;
            }
    }
	
    public void IncreaseNumber(int Increaser)
    {
        switch (Increaser)
        {
            case 1:
                {
                    ++MyNumber;
                    MyNumberText.text = MyNumber.ToString();
                    break;
                }
            case -1:
                {
                    --MyNumber;

                    if(MyNumber < 1)
                    {
                        MyNumber = 0;
                    }

                    break;
                }
            case 0:
                {
                    MyNumber = 0;
                    break;
                }

            default:
                break;
        }

    }

}
