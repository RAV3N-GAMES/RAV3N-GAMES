using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ChangePopUp_Building : ChangePopUp
{
    public Text HPText;
    public Text LvText;

    public override void InitPopUp()
    {
        base.InitPopUp();

        HPText.text = objInfo.presentHP.ToString() + "/" + objInfo.totalHP.ToString();
        LvText.text = objInfo.level.ToString();

        BuildingObject building = JsonDataManager.GetBuildingInfo(id, objInfo.level);
        price = (building.Price / 2) * objInfo.presentHP / objInfo.totalHP;
        priceText.text = price.ToString();

        repairPrice = (objInfo.totalHP - objInfo.presentHP) * building.RepairCost;
        repairPriceText.text = repairPrice.ToString();
    }

    public override void tempHitObjForTest()
    {
        objInfo.SetHP(-20);
    }
}
