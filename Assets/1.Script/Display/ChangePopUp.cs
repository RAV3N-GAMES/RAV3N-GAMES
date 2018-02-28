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

    public void InitPopUp()
    {
        ObjectInfo objInfo = Obj.GetComponent<ObjectInfo>();
        levelText.text = objInfo.level.ToString();
        nameText.text = objInfo.id;
        priceText.text = "";
        repairPriceText.text = "";

        ObjImage.sprite = null;
    }

    public void RepairPref()
    {
        Obj.GetComponent<ObjectInfo>().RepairObject();
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
