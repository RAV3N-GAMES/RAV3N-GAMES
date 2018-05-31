using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskManager : MonoBehaviour
{
    public static TaskManager taskManager;

    public List<GameObject> Sign;

    public GameObject TaskObjectParent;
    TaskObject[] TaskObjectArray;

    public GameObject Dialog;
    public Transform Mask;

    public ResultPopUp resultPopUp;
    public DayandNight dayAndNight;
    public RoomManager roomManager;

    public GameObject ExpandMap;
    public GameObject OriginalMap;

    public CreateObject createObject;
    public Transform CreatePopUpParent;
    public CreatePopUp createPopUp;

    public Transform Display;
    List<GameObject> DisplayObject;

    public static bool isDone = true;

    [Range(0f, 2f)]
    public float clickTime;
    [Range(0f, 2f)]
    public float arrowSpeed;
    [Range(0f, 2f)]
    public float loopArrowSpeed;

    [HideInInspector]
    public bool isMove = false;
    [HideInInspector]
    public bool isPossibleClick = false;

    public enum Type
    {
        Slide,
        Select,
        LongSelect,
        Slide2,
        None
    }

    public void OnOffSign(Type type, bool isOn)
    {
        Sign[(int)type].SetActive(isOn);
    }

    void Awake()
    {
        if (taskManager == null)
            taskManager = this;
        RoomManager.ChangeClickStatus(false);

        DisplayObject = new List<GameObject>();

        int Cnt = Display.childCount;
        for (int i = 0; i < Cnt; i++)
            DisplayObject.Add(Display.GetChild(i).gameObject);

        TaskObjectArray = TaskObjectParent.GetComponents<TaskObject>();
    }

    public bool OnTask(int idx)
    {
        if(idx == 68)
        {
            ExpandMap.SetActive(true);
            OriginalMap.SetActive(false);

            return true;
        }
        else if(idx == 71)
        {
            ExpandMap.SetActive(false);
            OriginalMap.SetActive(true);
            OffDisplayObject();

            return true;
        }
        
        for (int i = 0; i < TaskObjectArray.Length; i++)
        {
            if (TaskObjectArray[i].Idx == idx)
            {
                Dialog.SetActive(false);

                OffDisplayObject();
                TaskObjectArray[i].InitTask(roomManager, resultPopUp, dayAndNight);
                return true;
            }
        }
        return false;
    }

    void OffDisplayObject()
    {
        for (int i = 0; i < DisplayObject.Count; i++)
        {
            if (DisplayObject[i].activeInHierarchy)
                DisplayObject[i].SetActive(false);
        }
    }

    Vector3 GetRay()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 0;
        Vector3 pos = Camera.main.ScreenToWorldPoint(mousePos);

        return pos;
    }

    void Update()
    {
        if (isPossibleClick != ClickObject.isPossibleClick)
            ClickObject.isPossibleClick = isPossibleClick;
        if (isMove)
        {
            if (RoomManager.possibleDrag)
            {
                if (Input.GetMouseButton(0))
                {
                    roomManager.conPos = GetRay();
                    if (roomManager.isCameraInRoom())
                        Camera.main.transform.position += (roomManager.prePos - roomManager.conPos);
                    roomManager.prePos = roomManager.conPos;
                }
            }
        }
    }

    void LateUpdate()
    {
        if (isMove)
        {
            if (RoomManager.possibleDrag)
            {
                roomManager.conPos = GetRay();
                roomManager.prePos = roomManager.conPos;
            }
        }
    }
}