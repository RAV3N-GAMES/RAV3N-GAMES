using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjInfo_Building : ObjInfoPopUp
{
    public Text HPText;
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

        if (level != 0)
        {
            HPText.text = JsonDataManager.GetBuildingInfo(id, level).HP.ToString();
        }

        LvText.text = level.ToString();

        int upgradeCost = base.GetUpgradeCost();
        if (upgradeCost != -1)
        {
            UpgradeIcon.gameObject.SetActive(true);
            UpgradeIcon.text = upgradeCost.ToString();
        }
        else
            UpgradeIcon.gameObject.SetActive(false);
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

        ActivationPrice.text = JsonDataManager.GetBuildingInfo(id, 1).ActiveCost.ToString();
    }
}
