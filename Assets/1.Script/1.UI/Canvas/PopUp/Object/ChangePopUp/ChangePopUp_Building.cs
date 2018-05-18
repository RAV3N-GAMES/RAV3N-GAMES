using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ChangePopUp_Building : ChangePopUp
{
    public Text HPText;
    public Text LvText;

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

    public override void InitPopUp()
    {
        base.InitPopUp();

        HPText.text = objInfo.presentHP.ToString() + "/" + objInfo.totalHP.ToString();
        LvText.text = objInfo.level.ToString();

        SetLVRect(LvText.gameObject.GetComponent<RectTransform>());

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
