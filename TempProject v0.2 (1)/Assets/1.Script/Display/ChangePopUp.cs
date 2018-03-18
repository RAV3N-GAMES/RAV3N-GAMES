using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangePopUp : MonoBehaviour {
    [HideInInspector]
    public GameObject Obj;

    public Text levelText;
    public Text nameText;
    public Text priceText;
    public Text repairPriceText;

    public Image ObjImage;

    public Text HPText;

    public GameObject LackOfCoin;
    public GameObject DontDestroy;

    void InitBuilding(ObjectInfo objInfo)
    {
        Building building = JsonDataManager.GetBuildingInfo(objInfo.id, objInfo.level);
        priceText.text = ((building.Price / 2) * objInfo.presentHP / objInfo.totalHP).ToString();
        repairPriceText.text = ((objInfo.totalHP - objInfo.presentHP) * building.RepairCost).ToString();
    }

    void InitOurForces(ObjectInfo objInfo)
    {
        OurForces ourForces = JsonDataManager.GetOurForcesInfo(objInfo.id, objInfo.level);
        priceText.text = ((ourForces.Price / 2) * objInfo.presentHP / objInfo.totalHP).ToString();
        repairPriceText.text = ((objInfo.totalHP - objInfo.presentHP) * ourForces.HealCost).ToString();
    }

    void InitTrap(ObjectInfo objInfo)
    {
        Trap trap = JsonDataManager.GetTrapInfo(objInfo.id, objInfo.level);
        priceText.text = (trap.Price / 2).ToString();
        repairPriceText.text = "0";
    }

    void InitSecret(ObjectInfo objInfo)
    {
        //Secret secret = JsonDataManager.GetSecretInfo(objInfo.id, Data_Player.Fame);
        priceText.text = "0";
        repairPriceText.text = "0";
    }


    public void InitPopUp()
    {
        ObjectInfo objInfo = Obj.GetComponent<ObjectInfo>();
        levelText.text = objInfo.level.ToString();
        nameText.text = objInfo.id;
        HPText.text = objInfo.presentHP.ToString() + "/" + objInfo.totalHP.ToString();

        switch (objInfo.type)
        {
            case 0:
                InitBuilding(objInfo);
                break;
            case 2:
                InitOurForces(objInfo);
                break;
            case 3://Trap
                InitTrap(objInfo);
                break;
            case 4://Secret
                InitSecret(objInfo);
                break;
            default:
                priceText.text = "0";
                repairPriceText.text = "0";
                break;
        }

        ObjImage.sprite = JsonDataManager.slotImage[objInfo.id];
    }

    public void RepairPref()
    {
        if (Data_Player.isEnough_G(int.Parse(priceText.text)))
        {
            Data_Player.subGold(int.Parse(repairPriceText.text));

            Obj.GetComponent<ObjectInfo>().RepairObject();

            InitPopUp();
        }
        else
            LackOfCoin.SetActive(true);
    }

    public void DestroyPref()
    {
        bool isDestroy = Obj.GetComponent<DisplayObject>().DestroyObj();

        if (isDestroy)
        {
            Data_Player.addGold(int.Parse(priceText.text));

            Destroy(Obj);
            PossibleDrag();
        }
        else
            DontDestroy.SetActive(true);

        gameObject.SetActive(false);
    }

    public void PossibleDrag()
    {
        RoomManager.possibleDrag = true;
        ClickObject.isPossibleClick = true;
    }
}
