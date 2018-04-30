using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatePopUp : MonoBehaviour {
    [HideInInspector]
    public GameObject Obj;

    public GameObject RotationButton;
    public GameObject LackOfCoin;

    public RoomManager roomManager;

    void UsingTile(string id, int price)
    {
        if (!id.Equals("Warp"))
        {
            Data_Player.subGold(price);
        }
        else
        {
            if (Obj.GetComponent<ObjectInfo>().id.Equals("Warp_Exit"))
                Data_Player.subGold(JsonDataManager.slotInfoList["Warp"].price);
            else
                Obj.transform.Find("Warp_Exit").gameObject.SetActive(true);
        }

        Obj.GetComponent<ObjectInfo>().OnDisplay();
    }

    public void CreatePref()
    {
        string id = Obj.GetComponent<ObjectInfo>().id;

        if (id.Equals("Warp_Exit"))
            id = "Warp";
        int price = JsonDataManager.slotInfoList[id].price;
        
        if (Data_Player.isEnough_G(price))
        {
            if (Obj.GetComponent<DisplayObject>().UsingTile())
                UsingTile(id, price);
            else
                CancelPref();

            if(!Obj.GetComponent<ObjectInfo>().id.Equals("Warp"))
                RoomManager.ChangeClickStatus(true);
        }
        else
        {
            Destroy(Obj);
            LackOfCoin.SetActive(true);
        }
        

        roomManager.SetClickColliderStatus(true);
        gameObject.SetActive(false);
    }

    IEnumerator CheckTile()
    {
        yield return null;
        Obj.GetComponent<CheckTile>().OnDisplayCheckTile();

        yield break;
    }

    public void RotationPref()
    {
        //코루틴 진행중이면 회전안하게 막아야지
        Obj.GetComponent<ObjectInfo>().rotationObject();

        StartCoroutine("CheckTile");
    }

    public void CancelPref()
    {
        if (Obj.GetComponent<ObjectInfo>().id.Equals("Warp_Exit"))
        {
            GameObject Warp = Obj.transform.parent.gameObject;
            bool temp = Warp.GetComponent<DisplayObject>().DestroyObj(false);

            print(temp);

            Destroy(Warp);
        }
        else
            Destroy(Obj);

        roomManager.SetClickColliderStatus(true);
        RoomManager.ChangeClickStatus(true);

        gameObject.SetActive(false);
    }
}
