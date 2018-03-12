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

    void InitBuilding(ObjectInfo objInfo)
    {
        Building building = JsonDataManager.GetBuildingInfo(objInfo.id, objInfo.level);
        priceText.text = building.Price.ToString();
        repairPriceText.text = ((objInfo.totalHP - objInfo.presentHP) * building.RepairCost).ToString();
    }

    void InitOurForces(ObjectInfo objInfo)
    {
        OurForces ourForces = JsonDataManager.GetOurForcesInfo(objInfo.id, objInfo.level);
        priceText.text = ourForces.Price.ToString();
        repairPriceText.text = ((objInfo.totalHP - objInfo.presentHP) * ourForces.HealCost).ToString();
    }

    void InitTrap(ObjectInfo objInfo)
    {
        Trap trap = JsonDataManager.GetTrapInfo(objInfo.id, objInfo.level);
        priceText.text = trap.Price.ToString();
        repairPriceText.text = "0";
    }

    void InitSecret(ObjectInfo objInfo)
    {
        Secret secret = JsonDataManager.GetSecretInfo(objInfo.id, objInfo.level);
        priceText.text = secret.Price.ToString();
        repairPriceText.text = "0";
    }


    public void InitPopUp()
    {
        ObjectInfo objInfo = Obj.GetComponent<ObjectInfo>();
        levelText.text = objInfo.level.ToString();
        nameText.text = objInfo.id;
        HPText.text = objInfo.presentHP.ToString() + "/" + objInfo.totalHP.ToString();

        switch (objInfo.type)
        { //가격은 임시 -> 계산하는걸로 바꿔야함
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

        //ObjImage.sprite = null;
    }

    public void RepairPref()
    {
        Obj.GetComponent<ObjectInfo>().RepairObject();
        InitPopUp();
        PossibleDrag();
    }

    public void DestroyPref()
    {
        Obj.GetComponent<DisplayObject>().DestroyObj();
        Destroy(Obj);
        gameObject.SetActive(false);

        PossibleDrag();
    }

    public void PossibleDrag()
    {
        RoomManager.possibleDrag = true;
    }
}
