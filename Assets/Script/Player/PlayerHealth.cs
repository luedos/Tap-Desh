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

        CharShooting MyCH = GetComponent<CharShooting>();
        if (MyCH != null)
        {
            MyCH.SetFireMultOnLevel(0);
            MyCH.MakeDD(0f);
        }

        CharMovement MyMV = GetComponent<CharMovement>();
        if (MyMV)
            MyMV.MakeBetterTP(0f);

        if (isInvincible)
            MakeInvincible(0f);

                
        GameManager.Instance.GameOver();

        base.Death();
    }

    public override void Start()
    {
        base.Start();
        CallHPSubs();
        MakeInvincible(3f);
    }

    public override void MakeAllive()
    {
        MobileInput MyMI = GetComponent<MobileInput>();
        if (MyMI != null)
            MyMI.BlockInput = false;

        base.MakeAllive();        
    }

}
