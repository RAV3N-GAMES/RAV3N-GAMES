using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour {
    public Transform RoomParent;
    public List<GameObject> Room { get; private set; }

    [HideInInspector]
    public int CenterRoomIdx;

    public static bool possibleDrag;
    bool isMove;

    Vector3 prePos;
    Vector3 conPos;

    Touch touch0;
    Touch touch1;

    float touchDistance;

    public static bool isTransparency;

    public MiniMapManager miniMapManager;

    void Awake()
    {
        possibleDrag = true;

        isMove = false;
        CenterRoomIdx = 0;

        Room = new List<GameObject>();

        for (int i = 0; i < RoomParent.childCount; i++)
        {
            Room.Add(RoomParent.GetChild(i).gameObject);
        }
    }

    public void MoveRoom(int idx)
    {
        Camera.main.transform.position = new Vector3(Room[idx].transform.position.x, Camera.main.transform.position.y, Room[idx].transform.position.z);
    }

    public void OpenRoom(int idx)
    {
        Room[idx].SetActive(true);
    }

    Vector3 GetRay()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 0;
        Vector3 pos = Camera.main.ScreenToWorldPoint(mousePos);

        return pos;
    }

    void MoveMapRoom()
    {
        int idx = 0;
        Vector3 cameraPos = Camera.main.transform.position;
        cameraPos.y = 0;

        for (int i = 0; i < Room.Count; i++)
        {
            if (Room[i].activeInHierarchy)
            {
                if ((cameraPos - Room[i].transform.position).magnitude < (cameraPos - Room[idx].transform.position).magnitude)
                    idx = i;
            }
        }

        CenterRoomIdx = idx;
    }

    public void ZoomInOut() //이거 직교로 수정해야함
    {
        touch0 = Input.GetTouch(0);
        touch1 = Input.GetTouch(1);

        if (touch0.phase == TouchPhase.Moved || touch1.phase == TouchPhase.Moved)
        {
            float dis = Vector3.Distance(touch0.position, touch1.position);
            Vector3 camPos = Camera.main.transform.position;

            if (dis < touchDistance && camPos.y < 15)
                Camera.main.transform.position = new Vector3(camPos.x, camPos.y + 0.1f, camPos.z);
            else if (dis > touchDistance && camPos.y > 3)
                Camera.main.transform.position = new Vector3(camPos.x, camPos.y - 0.1f, camPos.z);
        }
        touchDistance = Vector3.Distance(touch0.position, touch1.position);
    }

    public void OnTransparency()
    {
        isTransparency = !isTransparency;

        for (int i = 0; i < Room.Count; i++)
        {
            Room[i].GetComponent<TileManager>().OnTransparency(isTransparency);
        }
    }

    public void SetClickColliderStatus(bool isAcitve)
    {
        for (int i = 0; i < Room.Count; i++)
        {
            Room[i].GetComponent<TileManager>().SetClickColliderStatus(isAcitve);
        }
    }

    public static void ChangeClickStatus(bool isPossible)
    {
        print("ClickStatus : " + isPossible);
        possibleDrag = isPossible;
        ClickObject.isPossibleClick = isPossible;
    }

    public void ChangeClickStatusForButton(bool isPossible)
    {
        print("ClickStatus button : " + isPossible);
        possibleDrag = isPossible;
        ClickObject.isPossibleClick = isPossible;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && possibleDrag)
        {
            prePos = conPos = GetRay();
            isMove = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isMove = false;
            MoveMapRoom();

            miniMapManager.MoveMapRoom(CenterRoomIdx);
        }

        if (Input.touchCount == 2)
        {
            ZoomInOut();
        }
        //else if (Input.touchCount == 1)
        {
            if (isMove && possibleDrag)
            {
                conPos = GetRay();

                Camera.main.transform.position += (prePos - conPos);
                prePos = conPos;
            }
        }
    }
    void LateUpdate()
    {

        if (isMove && possibleDrag)
        {
            conPos = GetRay();
            prePos = conPos;
        }
    }
}
