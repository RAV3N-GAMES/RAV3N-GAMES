using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoxManager : MonoBehaviour {
    public GameObject SlotPref;

    Object[] LoadImage(string boxType)
    {
        Object[] loadImage = null;

        switch (boxType)
        {
            case "Wall":
            case "Trap":
            case "Monster":
            case "Treasure":
                loadImage = Resources.LoadAll("Image/" + boxType);
                break;
            default:
                loadImage = Resources.LoadAll("Image/Wall");
                break;
        }

        return loadImage;
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

    void InitSlot(int idx, GameObject LoadImage)
    {
        GameObject Slot = Instantiate(SlotPref, transform);
        InitSlotRect(Slot.GetComponent<RectTransform>(), idx);

        Slot.GetComponent<SlotManager>().InitSlotInfo(LoadImage, GetComponent<CreateObject>());
    }

    void InitBox()
    {
        DestroySlot();
        GetComponent<RectTransform>().offsetMin = Vector2.zero;
        GetComponent<RectTransform>().offsetMax = Vector2.zero;
    }

    public void LoadSlot(string boxType)
    {
        InitBox();
        Object[] loadImage = LoadImage(boxType);

        for (int i = 0; i < loadImage.Length; i++)
        {
            InitSlot(i, loadImage[i] as GameObject);
        }
    }
}
