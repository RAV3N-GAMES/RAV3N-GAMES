using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjInfoPopUp : MonoBehaviour {
    [HideInInspector]
    public string id;
    [HideInInspector]
    public SlotManager slotManager;

    public GameObject LackOfCoin;

    int GetUpgradeCost()
    {
        SlotInfo slotInfo = JsonDataManager.slotInfoList[id];
        int type = slotInfo.type;

        switch (type)
        {
            case 0:
                return JsonDataManager.GetBuildingInfo(id, slotInfo.level).UpgradeCost;
            case 2:
                return JsonDataManager.GetOurForcesInfo(id, slotInfo.level).UpgradeCost;
            case 3:
                return JsonDataManager.GetTrapInfo(id, slotInfo.level).UpgradeCost;
            default:
                return -1;
        }
    }

	public void LevelUp()
    {
        int upgradeCost = GetUpgradeCost();

        if (upgradeCost == -1)
            return;

        if (Data_Player.isEnough_G(upgradeCost))
        {
            Data_Player.subGold(upgradeCost);

            JsonDataManager.SetSlotInfo(JsonDataManager.slotInfoList[id].type, id, JsonDataManager.slotInfoList[id].level + 1);
            slotManager.RefreshInfo();
        }
        else
            LackOfCoin.SetActive(true);
    }
}
