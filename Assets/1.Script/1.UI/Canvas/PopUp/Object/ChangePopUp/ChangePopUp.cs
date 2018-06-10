using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangePopUp : MonoBehaviour {
    [HideInInspector]
    public GameObject Obj;

    protected string id;
    
    protected int price;
    protected int repairPrice;

    public Text priceText;
    public Text repairPriceText;
    public Text partialRepairPriceText;

    public Image ObjImage;

    public GameObject PartialRepair;
    public GameObject DontDestroy;

    public GameObject MovePopUp;
    bool isMove;
    
    protected ObjectInfo objInfo;


    public virtual void InitPopUp()
    {
        objInfo = Obj.GetComponent<ObjectInfo>();
        if (objInfo.id.Equals("Warp_Exit"))
            id = "Warp";
        else
            id = objInfo.id;
        
        partialRepairPriceText.text = Data_Player.Gold.ToString();
        ObjImage.sprite = JsonDataManager.slotImage[id];
    }
    
    public void MovePref()
    {
        if (Obj.GetComponent<ObjectInfo>().DontDestroy == 0)
        {
            Obj.GetComponent<ObjectMove>().enabled = true;
            Obj.GetComponent<ObjectMove>().changePos = true;
            Obj.GetComponent<ObjectInfo>().isDisplay = false;
            Obj.GetComponent<ObjectColor>().OnColor(true);
            Obj.GetComponent<DisplayObject>().CreateButton = MovePopUp;
            isMove = true;
            Obj.GetComponent<ObjectInfo>().TileCollider.SetActive(true);
        }
        else
            DontDestroy.SetActive(true);
    }

    public void PartialRepairPref()
    {
        ObjectInfo objInfo = Obj.GetComponent<ObjectInfo>();
        int repairHP = (int)((objInfo.totalHP - objInfo.presentHP) * ((float)Data_Player.Gold / repairPrice));

        Obj.GetComponent<ObjectInfo>().RepairObject(repairHP);
        Data_Player.subGold(Data_Player.Gold);

        SoundManager.soundManager.OnEffectSound("47_REPAIR");

        RoomManager.ChangeClickStatus(true);
    }

    public void RepairPref()
    {
        if (Data_Player.isEnough_G(repairPrice))
        {
            Data_Player.subGold(repairPrice);
            Obj.GetComponent<ObjectInfo>().RepairObject(-1);

            SoundManager.soundManager.OnEffectSound("47_REPAIR");

            RoomManager.ChangeClickStatus(true);
        }
        else
            PartialRepair.SetActive(true);
    }

    public virtual void tempHitObjForTest()
    {
        ;
    }

    void DestroyWarp()
    {
        if (Obj.GetComponent<ObjectInfo>().id.Equals("Warp_Exit"))//Warp 자식일 경우 자기 부모 디스트로이
        {
            GameObject Warp = Obj.transform.parent.gameObject;
            Warp.GetComponent<DisplayObject>().DestroyObj(false);

            Destroy(Warp);
        }
        else if (Obj.GetComponent<ObjectInfo>().id.Equals("Warp"))
        {
            Obj.transform.Find("Warp_Exit").gameObject.GetComponent<DisplayObject>().DestroyObj(false);
        }

    }

    public void DestroyPref()
    {
        bool isDestroy = Obj.GetComponent<DisplayObject>().DestroyObj(false);

        if (isDestroy)
        {
            DestroyWarp();
            string Id = Obj.GetComponent<ObjectInfo>().id;
            if (Id.Equals("UFOCore") || Id.Equals("SpaceVoiceRecordingFile") || Id.Equals("AlienBloodStorage") || Id.Equals("AlienStorageCapsule"))
            {
                SecretActs s = Obj.GetComponentInChildren<SecretActs>(true);
                SecretManager.SecretList.Remove(s);
                SecretManager.SecretCount--;
            }
            Data_Player.addGold(price);
            RoomManager.ChangeClickStatus(true);
            Destroy(Obj);
        }
        else
            DontDestroy.SetActive(true);

        gameObject.SetActive(false);
    }

    void OnEnable()
    {
        isMove = false;
    }

    void OnDisable()
    {
        if (!isMove)
        {
            try
            {
                Obj.GetComponent<ObjectColor>().OnRecognizeRage(false);
            }
            catch (System.Exception e) { print(e.Message); }
        }
    }
}
