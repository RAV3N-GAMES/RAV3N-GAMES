using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
    public bool InitialRooms(int idx) {
        return (idx <= 1 || idx == 5 || idx == 6) ? true : false;//idx= 0 or 1 or 5 or 6 -> true
    }

    public void OpenRoom(int idx)
    {
        Room[idx].SetActive(true);
        if (idx >= 1) { 
            if (Room[idx - 1].gameObject.activeSelf) {
                NavMeshObstacle[] obs = Room[idx - 1].GetComponentsInChildren<NavMeshObstacle>();
                Debug.Log("Name0: " + obs[0].name);
                Debug.Log("Name1: " + obs[1].name);

                obs[1].enabled = false;
            }
        }
        if (idx >= 5) { 
            if (Room[idx - 5].gameObject.activeSelf) {
                NavMeshObstacle[] obs = Room[idx - 5].GetComponentsInChildren<NavMeshObstacle>();
                Debug.Log("Name0: " + obs[0].name);
                Debug.Log("Name1: " + obs[1].name);
                Debug.Log("Name P: " + obs[0].transform.parent.transform.parent.name);
                obs[0].enabled = false;
            }
        }
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
            //Vector3 camPos = Camera.main.transform.position;
            float camSize = Camera.main.orthographicSize;

            if (dis < touchDistance && camSize < 15)
                Camera.main.orthographicSize = camSize + 0.1f;
            //Camera.main.transform.position = new Vector3(camPos.x, camPos.y + 0.1f, camPos.z);
            else if (dis > touchDistance && camSize > 3)
                Camera.main.orthographicSize = camSize - 0.1f;
            //Camera.main.transform.position = new Vector3(camPos.x, camPos.y - 0.1f, camPos.z);
        }
        touchDistance = Vector3.Distance(touch0.position, touch1.position);
    }

    public bool AllRepair()
    {
        int repairCost = GetAllRepairCost();
        if (!Data_Player.isEnough_G(repairCost))
            return false;

        Data_Player.subGold(repairCost);
        for (int i = 0; i < Room.Count; i++)
            Room[i].GetComponent<TileManager>().AllRepair();
        return true;
    }

    public int GetAllRepairCost()
    {
        int allRepairCost = 0;
        for(int i = 0; i < Room.Count; i++)
        {
            if (Room[i].activeInHierarchy)
                allRepairCost += Room[i].GetComponent<TileManager>().GetRepairCost();
        }

        return allRepairCost;
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
