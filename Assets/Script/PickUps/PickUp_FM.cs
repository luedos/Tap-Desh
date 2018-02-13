using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp_FM : PickUp {

    public int MaxPoints = 30;
    public int MinPoints = 20;

    public int MaxFireMode = 3;
    public int MaxHPBuffer = 3;
    public int MaxTPDamage = 2;

    public override void PickUpMe(GameObject byObject)
    {
        int MyNum = Random.Range(1, 4);
        switch (MyNum)
        {
            case 1:
                GoFireMode(byObject);
                break;
            case 2:
                GoTPDamage(byObject);
                break;
            case 3:
                GoHPBuffer(byObject);
                break;
            default:
                GoPoints();
                break;
        }
    }

    private void GoFireMode(GameObject inPlayer)
    {
        CharShooting MyShooting = inPlayer.GetComponent<CharShooting>();
        if (MyShooting != null)
        {
            if(MyShooting.GetFireMode >= MaxFireMode)
            {
                GoPoints();
                return;
            }
            
            MyShooting.SetFireMultOnLevel(5);
            GameManager.Instance.myHUD.GetComponent<HUDScript>().ShowMessage("Fire mode on level " + MyShooting.GetFireMode);
            Destroy(gameObject);
        }
    }

    private void GoHPBuffer(GameObject inPlayer)
    {
        PlayerHealth MyHP = inPlayer.GetComponent<PlayerHealth>();
        if(MyHP != null)
        {
            if(MyHP.LimitHP - MyHP.StartMaxHP >= MaxHPBuffer)
            {
                GoPoints();
                return;
            }

            ++MyHP.LimitHP;
            MyHP.RegenHP(1);
            MyHP.CallHPSubs();
            GameManager.Instance.myHUD.GetComponent<HUDScript>().ShowMessage("Player max HP now " + MyHP.LimitHP);
            Destroy(gameObject);
        }
    }

    private void GoTPDamage(GameObject inPlayer)
    {
        CharMovement MyCM = inPlayer.GetComponent<CharMovement>();
        if(MyCM != null)
        {
            if(MyCM.TPDamage >= MaxTPDamage)
            {
                GoPoints();
                return;
            }

            ++MyCM.TPDamage;
            if(MyCM.TPDamage == 1)
                GameManager.Instance.myHUD.GetComponent<HUDScript>().ShowMessage("TP now damaging enemies");
            else
                GameManager.Instance.myHUD.GetComponent<HUDScript>().ShowMessage("TP damaging on " + MyCM.TPDamage);
            Destroy(gameObject);
        }

    }

    private void GoPoints()
    {
        int MyNum = Random.Range(MinPoints, MaxPoints + 1);

        GameManager.Instance.IncreaseGamePoints(MyNum);

        GameManager.Instance.myHUD.GetComponent<HUDScript>().ShowMessage("Points bonus " + MyNum);

        Destroy(gameObject);
    }

    // Set up fire mode
    void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.tag == "Player")
        {
            PickUpMe(other.gameObject);
        }

    }
}
