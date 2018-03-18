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
    public Text ActivationButton;
    

    [HideInInspector]
    public string id;
    [HideInInspector]
    public int type;

    public void InitSlotInfo(string id, CreateObject createObject)
    {
        this.id = id;

        RefreshInfo();
        SlotInfo slotInfo = JsonDataManager.slotInfoList[id];
        slotImage.sprite = JsonDataManager.slotImage[slotInfo.imageName];

        GetComponent<ClickSlot>().id = slotInfo.id;
        GetComponent<ClickSlot>().createObject = createObject;
        
        glassButton.onClick.AddListener(OnExplain);

        type = slotInfo.type;

        switch (type)
        {
            case 0:
                ActivationButton.text = "개발";
                break;
            case 2:
                ActivationButton.text = "고용";
                break;
            case 3:
                ActivationButton.text = "연구";
                break;
            case 4:
                InitSecretActBtn();
                break;
            default:
                ActivationButton.text = "활성화";
                break;

        }
        
        PopUp = GameObject.Find("Canvas").transform.Find("PopUp").transform.Find("ObjInfo").gameObject;
    }

    public void OnExplain()
    {
        ObjInfoPopUp objInfoPopUp = PopUp.GetComponent<ObjInfoPopUp>();
        objInfoPopUp.id = id;
        objInfoPopUp.slotManager = this;

        ClickObject.isPossibleClick = false;
        RoomManager.possibleDrag = false;

        PopUp.SetActive(true);
    }

    public void InitSecretActBtn()
    {
        //Fame 올라갈때 호출해줘야 함
        Secret tmpSecret = JsonDataManager.GetSecretInfo(id, Data_Player.Fame);
        double SecretBanditsGenChance = 0;

        if (tmpSecret != null)
            SecretBanditsGenChance = tmpSecret.SecretBanditsGenChance;

        if (SecretBanditsGenChance == 0)
        {
            BlockPanel.SetActive(true);
            ActivationButton.text = "보안등급\n" + SecretManager.SecretFame[id] + "등급";
        }
        else
        {
            BlockPanel.SetActive(false);
        }
    }

    public void OnActivationSlot()
    {
        if (type != 4)       //Secret의 경우 플레이어 명성이 오름에 따라 알아서 호출하도록
        {
            //여기에서 돈 체크
            BlockPanel.SetActive(false);

            JsonDataManager.SetSlotInfo(JsonDataManager.slotInfoList[id].type, id, 1);
            RefreshInfo();
        }
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
