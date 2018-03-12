using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotManager : MonoBehaviour {
    public Image slotImage;
    public Text price;
    public Button glassButton;

    GameObject PopUp;

    public GameObject BlockPanel;

    [HideInInspector]
    public string id;

    public void InitSlotInfo(string id, CreateObject createObject)
    {
        this.id = id;

        RefreshInfo();
        SlotInfo slotInfo = JsonDataManager.slotInfoList[id];
        slotImage.sprite = JsonDataManager.slotImage[slotInfo.imageName];

        GetComponent<ClickSlot>().id = slotInfo.id;
        GetComponent<ClickSlot>().createObject = createObject;
        
        glassButton.onClick.AddListener(OnExplain);
        
        PopUp = GameObject.Find("Canvas").transform.Find("PopUp").transform.Find("ObjInfo").gameObject;
    }

    public void OnExplain()
    {
        ObjInfoPopUp objInfoPopUp = PopUp.GetComponent<ObjInfoPopUp>();
        objInfoPopUp.id = id;
        objInfoPopUp.slotManager = this;

        PopUp.SetActive(true);
    }

    public void OnActivationSlot()
    {
        BlockPanel.SetActive(false);

        JsonDataManager.SetSlotInfo(JsonDataManager.slotInfoList[id].type, id, 1);
        RefreshInfo();
    }
    
    public void RefreshInfo()
    {
        SlotInfo slotInfo = JsonDataManager.slotInfoList[id];
        price.text = slotInfo.price.ToString();

        if (JsonDataManager.slotInfoList[id].level == 0)
            BlockPanel.SetActive(true);
        else
            BlockPanel.SetActive(false);
    }
}
