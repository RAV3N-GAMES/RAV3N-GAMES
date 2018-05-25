using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateObject : MonoBehaviour
{
    [HideInInspector]
    public string id;

    public GameObject ChangePopUpManager;
    public GameObject CreatePopUp;

    public RectTransform BoxRect;
    float MaxY;

    bool isCreate;

    public RoomManager roomManager;

    public bool isTutorial = false;
    public TutorialTile tutorialTile;
    public TaskManager taskManager;

    void Awake()
    {
        id = "";
        MaxY = BoxRect.anchorMax.y;
        isCreate = false;
    }

    void SetBoxRect(float maxY)
    {
        BoxRect.anchorMax = new Vector2(BoxRect.anchorMax.x, maxY);

        BoxRect.localPosition = Vector3.one;

        BoxRect.offsetMin = Vector2.zero;
        BoxRect.offsetMax = Vector2.zero;
    }


    public void MouseUp()
    {
        if (isTutorial) {
            taskManager.isCompleted = tutorialTile.isSuccess();
            if (!taskManager.isCompleted)
                taskManager.StopCoroutine(taskManager.DisplayTask());
        }
        id = "";
        SetBoxRect(MaxY);
        isCreate = false;
    }

    void InitObject()
    {
        GameObject newObj = Instantiate(Resources.Load("Object/" + id) as GameObject);

        newObj.name = id;
        newObj.GetComponent<DisplayObject>().CreateButton = CreatePopUp;
        newObj.GetComponent<ClickObject>().ChangePopUpManager = ChangePopUpManager;
        newObj.GetComponent<ObjectInfo>().InitObject();

        if (id.Equals("Warp"))
        {
            GameObject warp_Exit = newObj.transform.Find("Warp_Exit").gameObject;
            warp_Exit.GetComponent<DisplayObject>().CreateButton = CreatePopUp;
            warp_Exit.GetComponent<ClickObject>().ChangePopUpManager = ChangePopUpManager;
            warp_Exit.GetComponent<ObjectInfo>().InitObject();

            warp_Exit.GetComponent<ObjectMove>().isNewObj = true;
        }

        newObj.GetComponent<ObjectMove>().isNewObj = true;

        newObj.GetComponent<ObjectMove>().StartCoroutine("ArrayObject");
    }

    public void MouseExit()
    {
        print("MouseExit");

        if (!DayandNight.isDay)
        {
            print(id + " : " + RoomManager.possibleDrag);
            if (id != "" && Input.GetMouseButton(0) && RoomManager.possibleDrag)
            {
                InitObject();
                RoomManager.ChangeClickStatus(false);

                roomManager.SetClickColliderStatus(false);

                SetBoxRect(BoxRect.anchorMin.y + 0.02f);
                isCreate = true;
                id = "";
            }
        }
    }

    void Update()
    {
        if(Input.GetMouseButtonUp(0))
        {
            if (BoxRect.anchorMax.y != MaxY)
                MouseUp();
        }
    }
}