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
    public Text Attack;

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
            case "FlameThrowingTrap": min = new Vector2(0.76f, minY); max = new Vector2(0.86f, maxY); break;
            case "HumanTrap": min = new Vector2(0.73f, minY); max = new Vector2(0.83f, maxY); break;
            case "ObstructMovementCurrent": min = new Vector2(0.75f, minY); max = new Vector2(0.85f, maxY); break;
            case "Warp": min = new Vector2(0.66f, minY); max = new Vector2(0.76f, maxY); break;
        }

        lvRect.anchorMin = min;
        lvRect.anchorMax = max;

        lvRect.offsetMin = Vector2.zero;
        lvRect.offsetMax = Vector2.zero;
    }

    public override void InitObjInfoPopUp(string id, int type, SlotManager slotManager)
    {
        base.InitObjInfoPopUp(id, type, slotManager);


        TrapObject trap;

        if (level != 0)
        {
            trap = JsonDataManager.GetTrapInfo(id, level);
            LvText.text = level.ToString();
        }
        else
        {
            trap = JsonDataManager.GetTrapInfo(id, 1);
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
        {
            UpgradeIcon.gameObject.SetActive(false);
        }

        DisassemblyTime.text = trap.DisassemblyTime.ToString();
        CoolTime.text = trap.CoolTime.ToString();

        switch (id)
        {
            case "FlameThrowingTrap": case "ObstructMovementCurrent":
                Attack.text = trap.Attack.ToString(); break;
            default:
                Attack.text = ""; break;
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

        for (int i = 0; i < ActivationButton.Count; i++)
            ActivationButton[i].SetActive(false);

        switch (type)
        {
            case 0: ActivationButton[0].SetActive(true); break;
            case 2: ActivationButton[1].SetActive(true); break;
            case 3: ActivationButton[2].SetActive(true); break;
        }

        ActivationPrice.text = JsonDataManager.GetTrapInfo(id, 1).ActiveCost.ToString();
    }
}
