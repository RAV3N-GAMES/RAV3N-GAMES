using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjInfoPopUp : MonoBehaviour {
    [HideInInspector]
    public string id;
    [HideInInspector]
    public SlotManager slotManager;

	public void LevelUp()
    {
        JsonDataManager.SetSlotInfo(JsonDataManager.slotInfoList[id].type, id, JsonDataManager.slotInfoList[id].level + 1);
        slotManager.RefreshInfo();
    }
}
