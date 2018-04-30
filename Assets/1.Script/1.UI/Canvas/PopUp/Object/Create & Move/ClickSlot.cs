using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickSlot : MonoBehaviour, IPointerDownHandler {
    public string id;
    public CreateObject createObject;

    SlotManager slotManager;

    void Awake()
    {
        slotManager = GetComponent<SlotManager>();
    }

    public void OnPointerDown(PointerEventData data)
    {
        SlotInfo slotInfo = JsonDataManager.slotInfoList[id];
        if (slotInfo.type == 4)
        {
            if (JsonDataManager.GetSecretInfo(id, Data_Player.Fame) == null)
                createObject.id = "";
            else
                createObject.id = id;
        }
        else if (slotInfo.level != 0)
            createObject.id = id;
        else
            createObject.id = "";
    }
}
