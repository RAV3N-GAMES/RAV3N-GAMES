using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjInfo_Trap : ObjInfoPopUp
{
    public Text DisassemblyTime;
    public Text CoolTime;
    public Text LvText;
    public Text UpgradePrice;

    public Text UpgradeIcon;

    public Text ConLevel;
    public Text NextLevel;

    public Image UpgradeImage;

    public Image ActivationImage;
    public Text ActivationPrice;

    public override void InitObjInfoPopUp(string id, int type, SlotManager slotManager)
    {
        base.InitObjInfoPopUp(id, type, slotManager);

        LvText.text = level.ToString();

        TrapObject trap = JsonDataManager.GetTrapInfo(id, level);
        


        int upgradeCost = base.GetUpgradeCost();
        if (upgradeCost != -1)
        {
            UpgradeIcon.gameObject.SetActive(true);
            UpgradeIcon.text = upgradeCost.ToString();

            DisassemblyTime.text = trap.DisassemblyTime.ToString();
            CoolTime.text = trap.CoolTime.ToString();
        }
        else
        {
            UpgradeIcon.gameObject.SetActive(false);

            DisassemblyTime.text = "";
            CoolTime.text = "";

        }
    }

    public void InitUpgradePopUp()
    {
        ConLevel.text = level.ToString();
        NextLevel.text = (level + 1).ToString();

        UpgradeImage.sprite = JsonDataManager.upgradeImage[id];

        UpgradePrice.text = UpgradeIcon.text;
    }

    public void InitActivationPopUp()
    {
        ActivationImage.gameObject.SetActive(true);
        ActivationImage.sprite = JsonDataManager.activationImage[id];

        ActivationPrice.text = JsonDataManager.GetTrapInfo(id, 1).ActiveCost.ToString();
    }
}
