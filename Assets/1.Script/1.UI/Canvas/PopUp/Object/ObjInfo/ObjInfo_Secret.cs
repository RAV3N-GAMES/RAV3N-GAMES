using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjInfo_Secret : ObjInfoPopUp {
    public Text SecretBanditsGenChance;

    public override void InitObjInfoPopUp(string id, int type, SlotManager slotManager)
    {
        base.InitObjInfoPopUp(id, type, slotManager);

        SecretObject secret = JsonDataManager.GetSecretInfo(id, Data_Player.Fame);
        Price.text = secret.Price.ToString();

        SecretBanditsGenChance.text = secret.SecretBanditsGenChance.ToString();
    }
}
