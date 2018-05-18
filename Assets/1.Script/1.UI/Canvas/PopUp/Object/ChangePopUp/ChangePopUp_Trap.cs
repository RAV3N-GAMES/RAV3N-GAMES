using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangePopUp_Trap : ChangePopUp
{
    public Text DisassemblyTime;
    public Text CoolTime;
    public Text LvText;
    public Text Attack;

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

    public override void InitPopUp()
    {
        base.InitPopUp();
        LvText.text = objInfo.level.ToString();
        SetLVRect(LvText.gameObject.GetComponent<RectTransform>());

        TrapObject trap = JsonDataManager.GetTrapInfo(id, objInfo.level);

        price = trap.Price / 2;
        priceText.text = price.ToString();

        repairPrice = 0;
        repairPriceText.text = "0";

        DisassemblyTime.text = trap.DisassemblyTime.ToString();
        CoolTime.text = trap.CoolTime.ToString();

        switch (id)
        {
            case "FlameThrowingTrap":
            case "ObstructMovementCurrent":
                Attack.text = trap.Attack.ToString(); break;
            default:
                Attack.text = ""; break;
        }
    }
}
