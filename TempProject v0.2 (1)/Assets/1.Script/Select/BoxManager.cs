using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoxManager : MonoBehaviour {
    public GameObject SlotPref;

    void Start()
    {
        LoadSlot(0);
    }

    void DestroySlot()
    {
        int Cnt = transform.childCount;

        for(int i= 0; i < Cnt; i++)
        {
            Destroy(transform.GetChild(i).gameObject);            
        }
    }

    void InitSlotRect(RectTransform SlotRect, int idx)
    {
        SlotRect.anchorMin = new Vector2(idx, 0.1f);
        SlotRect.anchorMax = new Vector2(idx + 1, 0.9f);

        SlotRect.offsetMax = Vector2.zero;
        SlotRect.offsetMin = Vector2.zero;
    }

    void InitSlot(int idx, string id)
    {
        GameObject Slot = Instantiate(SlotPref, transform);
        InitSlotRect(Slot.GetComponent<RectTransform>(), idx);

        Slot.GetComponent<SlotManager>().InitSlotInfo(id, GetComponent<CreateObject>());
    }

    void InitBox()
    {
        DestroySlot();
        GetComponent<RectTransform>().offsetMin = Vector2.zero;
        GetComponent<RectTransform>().offsetMax = Vector2.zero;
    }

    List<string> GetSlotIdArray(int boxType)
    {
        List<string> slotId = new List<string>();

        foreach (var slot in JsonDataManager.slotInfoList)
        {
            if (slot.Value.type == boxType)
                slotId.Add(slot.Value.id);
        }

        return slotId;
    }

    public void LoadSlot(int boxType)
    {
        InitBox();
        List<string> slotIdList = GetSlotIdArray(boxType);

        for (int i = 0; i < slotIdList.Count; i++)
        {
            InitSlot(i, slotIdList[i]);
        }
    }
}
