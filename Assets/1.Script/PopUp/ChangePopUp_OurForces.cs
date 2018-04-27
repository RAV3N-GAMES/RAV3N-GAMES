using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangePopUp_OurForces : ChangePopUp
{
    public Text HPText;
    public Text Attack;
    public Text HitConstrain;
    public Text LvText;

    public override void InitPopUp()
    {
        base.InitPopUp();

        HPText.text = objInfo.presentHP.ToString() + "/" + objInfo.totalHP.ToString();
        LvText.text = objInfo.level.ToString();

        OurForcesObject ourForces = JsonDataManager.GetOurForcesInfo(id, objInfo.level);

        price = (ourForces.Price / 2) * objInfo.presentHP / objInfo.totalHP;
        priceText.text = price.ToString();

        repairPrice = (int)((objInfo.totalHP - objInfo.presentHP) * ourForces.HealCost);
        repairPriceText.text = repairPrice.ToString();

        Attack.text = ourForces.Attack.ToString();
        HitConstrain.text = ourForces.HitConstrain.ToString();
    }

    public override void tempHitObjForTest()
    {
        objInfo.SetHP(-20);
    }
}
