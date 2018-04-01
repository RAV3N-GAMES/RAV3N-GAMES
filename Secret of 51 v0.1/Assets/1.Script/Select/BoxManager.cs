using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoxManager : MonoBehaviour {
    public List<GameObject> SlotList;
    public List<GameObject> ButtonList;
    public GameObject ClickBlockPanel;
    int type;

    void Start()
    {
        type = -1;
        //LoadSlot(0);
    }

    void AllOffSlotList()
    {
        for (int i = 0; i < SlotList.Count; i++)
        {
            SlotList[i].SetActive(false);
            ButtonList[i].SetActive(false);
        }
    }

    void OnSlotList(int idx)
    {
        AllOffSlotList();
        SlotList[idx].SetActive(true);
        ButtonList[idx].SetActive(true);
    }

    public void LoadSlot(int boxType)
    {
        if (type == boxType)
        {
            type = -1;
            AllOffSlotList();
            ClickBlockPanel.SetActive(false);
            return;
        }

        type = boxType;
        switch (boxType)
        {
            case 0:
                OnSlotList(0);
                break;
            case 2:
                OnSlotList(1);
                break;
            case 3:
                OnSlotList(2);
                break;
            case 4:
                OnSlotList(3);
                break;
            case 5:
                OnSlotList(4);
                ClickBlockPanel.SetActive(true);
                break;
        }
    }
}
