using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangePopUp_Secret : ChangePopUp {
    public Text SecretBanditsGenChance;

    public override void InitPopUp()
    {
        base.InitPopUp();

        SecretObject secret = JsonDataManager.GetSecretInfo(id, Data_Player.Fame);

        price = 0;
        repairPrice = 0;

        priceText.text = "0";
        repairPriceText.text = "0";

        SecretBanditsGenChance.text = secret.SecretBanditsGenChance.ToString();
    }
}
