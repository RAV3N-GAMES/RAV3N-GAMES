using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjInfo_OurForces : ObjInfoPopUp {
    public Text HPText;
    public Text Attack;
    public Text HitConstrain;
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
        if (level != 0)
        {
            HPText.text = JsonDataManager.GetOurForcesInfo(id, level).HP.ToString();
        }

        OurForcesObject ourForces = JsonDataManager.GetOurForcesInfo(id, level);
        

        

        int upgradeCost = base.GetUpgradeCost();
        if (upgradeCost != -1)
        {
            UpgradeIcon.gameObject.SetActive(true);
            UpgradeIcon.text = upgradeCost.ToString();

            Attack.text = ourForces.Attack.ToString();
            HitConstrain.text = ourForces.HitConstrain.ToString();
        }
        else
        {
            UpgradeIcon.gameObject.SetActive(false);

            Attack.text = "";
            HitConstrain.text = "";
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
        ActivationImage.transform.parent.gameObject.SetActive(true);
        ActivationImage.sprite = JsonDataManager.activationImage[id];

        ActivationPrice.text = JsonDataManager.GetOurForcesInfo(id, 1).ActiveCost.ToString();
    }
}
