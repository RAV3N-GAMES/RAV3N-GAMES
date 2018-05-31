using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatePopUp : MonoBehaviour
{
    [HideInInspector]
    public GameObject Obj;

    public GameObject RotationButton;
    public GameObject LackOfCoin;

    public RoomManager roomManager;

    public GameObject ExceedLimitAllot;

    public bool isTutorial = false;
    public TutorialTile tutorialTile;
    public TaskObject taskObject;

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

    void OnEffectSound_Create(int type)
    {
        switch (type)
        {
            case 0: SoundManager.soundManager.OnEffectSound("10_BUILDING SET"); break;
            case 2: SoundManager.soundManager.OnEffectSound("48_SOLDIER SET"); break;
            case 3: SoundManager.soundManager.OnEffectSound("50_TRAP SET"); break;
            case 4: SoundManager.soundManager.OnEffectSound("52_SECRET SET"); break;
            default: break;
        }
    }

    public void CreatePref()
    {
        string id = Obj.GetComponent<ObjectInfo>().id;

        if (isTutorial)
        {
            if (!tutorialTile.isSuccess()) // tutorial tile과 닿았는지
            {
                print("설치 안됨");
                CancelPref();
                gameObject.SetActive(false);
                return;
            }
            else
            {
                //taskManager.SetTutorialTile();
            }
        }

        if (id.Equals("Warp_Exit"))
            id = "Warp";
        int price = JsonDataManager.slotInfoList[id].price;

        if (Data_Player.isEnough_G(price))
        {
            if (!Obj.GetComponent<CheckTile>().tileManager.IsPossibleAllot(Obj.GetComponent<ObjectInfo>().type))
            {
                CancelPref();
                ExceedLimitAllot.SetActive(true);
                gameObject.SetActive(false);

                return;
            }
            if (Obj.GetComponent<DisplayObject>().UsingTile(true))
            {
                UsingTile(id, price);

                OnEffectSound_Create(Obj.GetComponent<ObjectInfo>().type);
            }
            else
                CancelPref();

            if (!Obj.GetComponent<ObjectInfo>().id.Equals("Warp"))
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
        if (isTutorial)
        {
            tutorialTile.lastColList.Clear();
            taskObject.SetSlotStatus(true);
            taskObject.SetWarpExit();
        }

        if (Obj.GetComponent<ObjectInfo>().id.Equals("Warp_Exit"))
        {
            GameObject Warp = Obj.transform.parent.gameObject;
            bool temp = Warp.GetComponent<DisplayObject>().DestroyObj(false);

            Destroy(Warp);
        }
        else
        {
            Destroy(Obj);
        }

        roomManager.SetClickColliderStatus(true);
        RoomManager.ChangeClickStatus(true);

        if (isTutorial)
            ClickObject.isPossibleClick = false;


        gameObject.SetActive(false);
    }
}
