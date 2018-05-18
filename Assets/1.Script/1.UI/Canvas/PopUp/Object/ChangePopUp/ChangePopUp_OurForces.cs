using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangePopUp_OurForces : ChangePopUp
{
    public Text HPText;
    public Text Attack;
    public Text SkillCool;
    public Text LvText;

    void SetLVRect(RectTransform lvRect)
    {
        Vector2 min = Vector2.zero, max = Vector2.zero;
        float minY = lvRect.anchorMin.y;
        float maxY = lvRect.anchorMax.y;

        switch (id)
        {
            case "BiochemistryUnit": min = new Vector2(0.77f, minY); max = new Vector2(0.9f, maxY); break;
            case "Guard": min = new Vector2(0.71f, minY); max = new Vector2(0.8f, maxY); break;
            case "QuickReactionForces": min = new Vector2(0.735f, minY); max = new Vector2(0.83f, maxY); break;
            case "Researcher": min = new Vector2(0.715f, minY); max = new Vector2(0.805f, maxY); break;
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

        OurForcesObject ourForces = JsonDataManager.GetOurForcesInfo(id, objInfo.level);

        price = (ourForces.Price / 2) * objInfo.presentHP / objInfo.totalHP;
        priceText.text = price.ToString();

        repairPrice = (int)((objInfo.totalHP - objInfo.presentHP) * ourForces.HealCost);
        repairPriceText.text = repairPrice.ToString();

        Attack.text = ourForces.Attack.ToString();
        SkillCool.text = ourForces.SkillCool.ToString();
    }

    public override void tempHitObjForTest()
    {
        objInfo.SetHP(-20);
    }
}
