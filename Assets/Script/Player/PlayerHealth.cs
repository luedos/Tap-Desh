using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : HealthPoints {



    protected override void Death()
    {
        // making all bonuses dissapiar on death

        MobileInput MyMI = GetComponent<MobileInput>();
        if (MyMI != null)
            MyMI.BlockInput = true;
                
        GameManager.Instance.GameOver();

        base.Death();
    }

    public override void Start()
    {
        base.Start();
        
        MakeInvincible(3f);

        CallHPSubs();
    }

    public override void ResetPart()
    {
        MobileInput MyMI = GetComponent<MobileInput>();
        if (MyMI != null)
            MyMI.BlockInput = false;

        MakeInvincible(3f);

        base.ResetPart();        
    }

    // set up invincibility
    public void MakeInvincible(float OnTime)
    {
        if (OnTime == 0f)
        {
            if (isInvincible)
            {
                InvinsibilityTimer = 0f;
                isInvincible = false;
                GameManager.Instance.myHUD.GetComponent<HUDScript>().AddBonusIcon_Inv(0f);
            }

            return;
        }

        InvinsibilityTimer = OnTime;
        isInvincible = true;
        GameManager.Instance.myHUD.GetComponent<HUDScript>().AddBonusIcon_Inv(OnTime);
    }

}
