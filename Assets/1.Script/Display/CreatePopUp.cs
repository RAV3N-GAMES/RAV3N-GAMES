using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatePopUp : MonoBehaviour {
    [HideInInspector]
    public GameObject Obj;

    public GameObject LackOfCoin;

    public void CreatePref()
    {
        int price = JsonDataManager.slotInfoList[Obj.GetComponent<ObjectInfo>().id].price;

        if (Data_Player.isEnough_G(price))
        {
            if (Obj.GetComponent<DisplayObject>().UsingTile())
            {
                Data_Player.subGold(price);

                Obj.GetComponent<ObjectInfo>().OnDisplay();
            }
            else
                Destroy(Obj);

            PossibleDrag();
        }
        else
        {
            Destroy(Obj);
            LackOfCoin.SetActive(true);
        }

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
        Destroy(Obj);
        gameObject.SetActive(false);

        PossibleDrag();
    }

    public void PossibleDrag()
    {
        RoomManager.possibleDrag = true;
        ClickObject.isPossibleClick = true;
    }
}
