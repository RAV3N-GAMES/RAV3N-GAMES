using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangePopUp : MonoBehaviour {
    [HideInInspector]
    public GameObject Obj;

    string id;
    
    int price;
    int repairPrice;

    public Text nameText;
    public Text priceText;
    public Text repairPriceText;
    public Text contentsText;

    public Image ObjImage;

    public Text HPText;

    public GameObject PartialRepair;
    public GameObject DontDestroy;

    public GameObject MovePopUp;

    void InitBuilding(ObjectInfo objInfo)
    {
        BuildingObject building = JsonDataManager.GetBuildingInfo(id, objInfo.level);
        price = (building.Price / 2) * objInfo.presentHP / objInfo.totalHP;
        priceText.text = price.ToString() + "/"+ Data_Player.Gold + "골드회수";

        repairPrice = (objInfo.totalHP - objInfo.presentHP) * building.RepairCost;
        repairPriceText.text = repairPrice.ToString() + "/" + Data_Player.Gold + "골드";
    }

    void InitOurForces(ObjectInfo objInfo)
    {
        OurForcesObject ourForces = JsonDataManager.GetOurForcesInfo(id, objInfo.level);

        price = (ourForces.Price / 2) * objInfo.presentHP / objInfo.totalHP;
        priceText.text = price.ToString() + "/" + Data_Player.Gold + "골드회수";

        repairPrice = (int)((objInfo.totalHP - objInfo.presentHP) * ourForces.HealCost);
        repairPriceText.text = repairPrice.ToString() + "/" + Data_Player.Gold + "골드";
    }

    void InitTrap(ObjectInfo objInfo)
    {        
        TrapObject trap = JsonDataManager.GetTrapInfo(id, objInfo.level);

        price = trap.Price / 2;
        priceText.text = price.ToString() + "/" + Data_Player.Gold + " 골드회수";

        repairPrice = 0;
        repairPriceText.text = "0" + "/" + Data_Player.Gold + "골드";
    }

    void InitSecret(ObjectInfo objInfo)
    {
        price = 0;
        repairPrice = 0;

        priceText.text = "삭제 불가";
        repairPriceText.text = "0";
    }


    public void InitPopUp()
    {
        ObjectInfo objInfo = Obj.GetComponent<ObjectInfo>();

        if (objInfo.id.Equals("Warp_Exit"))
            id = "Warp";
        else
            id = objInfo.id;

        nameText.text = id + " " + objInfo.level.ToString() + "단계";
        HPText.text = "HP " + objInfo.presentHP.ToString() + "/" + objInfo.totalHP.ToString();
        contentsText.text = objInfo.level.ToString() +"단계의 " + id + "이다.";

        switch (objInfo.type)
        {
            case 0: InitBuilding(objInfo); break;
            case 2: InitOurForces(objInfo); break;
            case 3: InitTrap(objInfo);  break;
            case 4: InitSecret(objInfo); break;
            default:
                price = 0;
                repairPrice = 0;
                priceText.text = "0";
                repairPriceText.text = "0";
                break;
        }

        ObjImage.sprite = JsonDataManager.slotImage[id];
    }
    
    public void MovePref()
    {
        if (Obj.GetComponent<ObjectInfo>().DontDestroy == 0)
        {
            Obj.GetComponent<ObjectMove>().enabled = true;
            Obj.GetComponent<ObjectMove>().changePos = true;

            Obj.GetComponent<ObjectColor>().OnColor(true);
            Obj.GetComponent<DisplayObject>().CreateButton = MovePopUp;
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

        RoomManager.ChangeClickStatus(true);
    }

    public void RepairPref()
    {
        if (Data_Player.isEnough_G(repairPrice))
        {
            Data_Player.subGold(repairPrice);
            Obj.GetComponent<ObjectInfo>().RepairObject(-1);

            RoomManager.ChangeClickStatus(true);
        }
        else
            PartialRepair.SetActive(true);
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
        if (Obj.GetComponent<ObjectInfo>().type == 4)
            RoomManager.ChangeClickStatus(true);
        else
        {
            bool isDestroy = Obj.GetComponent<DisplayObject>().DestroyObj(false);

            if (isDestroy)
            {
                DestroyWarp();

                Data_Player.addGold(price);
                RoomManager.ChangeClickStatus(true);
                Destroy(Obj);
            }
            else
                DontDestroy.SetActive(true);
        }
        gameObject.SetActive(false);
    }
}
