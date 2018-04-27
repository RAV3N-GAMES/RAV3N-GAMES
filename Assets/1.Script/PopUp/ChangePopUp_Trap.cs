using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangePopUp_Trap : ChangePopUp
{
    public Text DisassemblyTime;
    public Text CoolTime;
    public Text LvText;

    public override void InitPopUp()
    {
        base.InitPopUp();
        LvText.text = objInfo.level.ToString();

        TrapObject trap = JsonDataManager.GetTrapInfo(id, objInfo.level);

        price = trap.Price / 2;
        priceText.text = price.ToString();

        repairPrice = 0;
        repairPriceText.text = "0";

        DisassemblyTime.text = trap.DisassemblyTime.ToString();
        CoolTime.text = trap.CoolTime.ToString();
    }
}
