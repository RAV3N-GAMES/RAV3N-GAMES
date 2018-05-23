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

    public List<GameObject> ActivationButton;

    void SetLVRect(RectTransform lvRect)
    {
        Vector2 min = Vector2.zero, max = Vector2.zero;
        float minY = lvRect.anchorMin.y;
        float maxY = lvRect.anchorMax.y;
        switch (id)
        {
            case "OldBuilding": min = new Vector2(0.77f, minY); max = new Vector2(0.9f, maxY); break;
            case "NewBuilding": min = new Vector2(0.725f, minY); max = new Vector2(0.855f, maxY); break;
            case "FunctionalBuilding": min = new Vector2(0.775f, minY); max = new Vector2(0.905f, maxY); break;
            case "CoreBuilding": min = new Vector2(0.73f, minY); max = new Vector2(0.86f, maxY); break;
        }

        lvRect.anchorMin = min;
        lvRect.anchorMax = max;

        lvRect.offsetMin = Vector2.zero;
        lvRect.offsetMax = Vector2.zero;
    }

    public override void InitObjInfoPopUp(string id, int type, SlotManager slotManager)
    {
        base.InitObjInfoPopUp(id, type, slotManager);

        if (level != 0)
        {
            HPText.text = JsonDataManager.GetBuildingInfo(id, level).HP.ToString();
            LvText.text = level.ToString();
        }
        else
        {
            HPText.text = JsonDataManager.GetBuildingInfo(id, 1).HP.ToString();
            LvText.text = "1";
        }

        SetLVRect(LvText.gameObject.GetComponent<RectTransform>());

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

        for (int i = 0; i < ActivationButton.Count; i++)
            ActivationButton[i].SetActive(false);

        switch (type)
        {
            case 0: ActivationButton[0].SetActive(true); break;
            case 2: ActivationButton[1].SetActive(true); break;
            case 3: ActivationButton[2].SetActive(true); break;
        }

        ActivationPrice.text = JsonDataManager.GetBuildingInfo(id, 1).ActiveCost.ToString();
    }
}
