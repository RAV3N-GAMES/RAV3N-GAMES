using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotManager : MonoBehaviour {
    public Text price;
    //public Button glassButton;

    GameObject PopUp;

    public GameObject BlockPanel;
    
    public string id;
    public int type;

    public GameObject LackOfCoin;

    public void Start()
    {
        if(type == 4)
            InitSecretActBtn();
        
        PopUp = GameObject.Find("Canvas").transform.Find("PopUp").transform.Find("ObjInfo").gameObject;
        
        RefreshInfo();
    }

    public void OnExplain()
    {
        ObjInfoPopUp objInfoPopUp = PopUp.GetComponent<ObjInfoPopUp>();

        objInfoPopUp.InitObjInfoPopUp(id, type, this);

        RoomManager.ChangeClickStatus(false);
        PopUp.SetActive(true);
    }

    public void InitSecretActBtn()
    {
        //Fame 올라갈때 호출해줘야 함
        SecretObject tmpSecret = JsonDataManager.GetSecretInfo(id, Data_Player.Fame);
        double SecretBanditsGenChance = 0;

        if (tmpSecret != null)
            SecretBanditsGenChance = tmpSecret.SecretBanditsGenChance;

        if (SecretBanditsGenChance == 0)
        {
            BlockPanel.SetActive(true);
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

        if (type != 4)
        {
            if (slotInfo.level == 0)
                BlockPanel.SetActive(true);
            else
                BlockPanel.SetActive(false);
        }
    }
}
