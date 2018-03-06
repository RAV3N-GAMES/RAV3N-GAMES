using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotManager : MonoBehaviour {
    public Image slotImage;
    public Text price;
    public Button glassButton;

    GameObject PopUp;

    [HideInInspector]
    public string id;

    public void InitSlotInfo(string id, CreateObject createObject)
    {
        this.id = id;
        SlotInfo slotInfo = JsonDataManager.slotInfoList[id];

        slotImage.sprite = JsonDataManager.slotImage[slotInfo.imageName];

        price.text = slotInfo.price.ToString();
        
        GetComponent<ClickSlot>().id = slotInfo.id;
        GetComponent<ClickSlot>().createObject = createObject;
        
        glassButton.onClick.AddListener(OnExplain);
        
        PopUp = GameObject.Find("Canvas").transform.Find("PopUp").gameObject;
    }

    public void OnExplain()
    {
        PopUp.SetActive(true);
    }
}
