using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class RoomManager : MonoBehaviour {
    public Transform RoomParent;
    public List<GameObject> Room { get; private set; }
    public List<bool> IsOutside { get; private set; }
    private const int MaxSide = 5;//한 변 최대 방 개수
    [HideInInspector]
    public int CenterRoomIdx;

    public static bool possibleDrag;
    bool isMove;

    [HideInInspector]
    public Vector3 prePos;
    [HideInInspector]
    public Vector3 conPos;

    Touch touch0;
    Touch touch1;

    float touchDistance;

    public static bool isTransparency;
    public MiniMapManager miniMapManager;
    /*private void checkOutside() {
        IsOutside[0] = false;
        int idx_i, idx_j;
        int idxAdjLefttop, idxAdjRighttop;
        for (int i = 1; i < Room.Count-MaxSide-1; i++) {
            idx_i = i / MaxSide;
            idx_j = i / MaxSide;
            idxAdjLefttop = (idx_i + 1) * MaxSide + idx_j;
            idxAdjRighttop = i * MaxSide + idx_j + 1;
            if (!Room[idxAdjLefttop].activeSelf || !Room[idxAdjRighttop].activeSelf)
            {
                IsOutside[i] = true;
            }
            else
                IsOutside[i] = false;
        }

        for (int i = 1; i < MaxSide; i++) {//4 9 14 19에 대해 outside 설정
            if (Room[MaxSide * i - 1].activeSelf)
                IsOutside[MaxSide * i - 1] = true;
            else
                IsOutside[MaxSide * i - 1] = false;
        }

        for (int i = 0; i < MaxSide; i++) {//20~24에 대해 outside 설정
            if (Room[(MaxSide * (MaxSide - 1)) + i].activeSelf)
                IsOutside[(MaxSide * (MaxSide - 1)) + i] = true;
            else
                IsOutside[(MaxSide * (MaxSide - 1)) + i] = false;
        }
    }
    */
    private void RemoveGenPoint() {

    }

    void Awake()
    {
        possibleDrag = true;

        isMove = false;
        CenterRoomIdx = 0;

        Room = new List<GameObject>();
        IsOutside = new List<bool>();
        for (int i = 0; i < RoomParent.childCount; i++)
        {
            Room.Add(RoomParent.GetChild(i).gameObject);
            IsOutside.Add(new bool());
        }
    }

    void Start() {
        //  checkOutside();
        ChangeClickStatus(true);

        SoundManager.soundManager.ChangeBGM("7_NIGHT");
    }

    public int[] GetObjectCnt(int idx)
    {
        return Room[idx].GetComponent<TileManager>().GetObjectCntArray();
    }

    public void MoveRoom(int idx)
    {
        Camera.main.transform.position = new Vector3(Room[idx].transform.position.x, Camera.main.transform.position.y, Room[idx].transform.position.z);
    }

    public bool InitialRooms(int idx) {
        return (idx <= 1 || idx == 5 || idx == 6) ? true : false;//idx= 0 or 1 or 5 or 6 -> true
    }

    private void SetObstacles(int idx) {
        int i = idx / MaxSide;
        int j = idx % MaxSide;

        int adjacentRoom = idx - 1;//열린 방과 인접한 방
        int adj_i = adjacentRoom / MaxSide;//왼쪽 아래 방부터 시작
        int adj_j = adjacentRoom % MaxSide;
        NavMeshObstacle[] obs_open;
        NavMeshObstacle[] obs_adj;
        Transform deleteform;

        if (idx > 1 && adj_i == i && adj_j == (j - 1) && Room[adjacentRoom].gameObject.activeSelf)
        {//열린 방의 왼쪽 아래에 방이 있을 경우
            obs_open = Room[idx].GetComponentsInChildren<NavMeshObstacle>();
            obs_adj = Room[adjacentRoom].GetComponentsInChildren<NavMeshObstacle>();
            obs_open[3].enabled = false;
            obs_adj[2].enabled = false;
            obs_open = null;
            obs_adj = null;
            deleteform = Room[adjacentRoom].transform.GetChild(19).GetChild(10);
            Room[adjacentRoom].GetComponent<EnemyGroup>().GenPoint.Remove(deleteform);
            deleteform = Room[idx].transform.GetChild(2).GetChild(10);
            Room[idx].GetComponent<EnemyGroup>().GenPoint.Remove(deleteform);
        }
        adjacentRoom = idx + 1;//오른쪽 위 방
        adj_i = adjacentRoom / MaxSide;
        adj_j = adjacentRoom % MaxSide;
        if (idx < Room.Count - 1 && adj_i == i && adj_j == j + 1 && Room[adjacentRoom].gameObject.activeSelf)
        {//열린 방의 오른쪽 위에 방이 있을 경우
            obs_open = Room[idx].GetComponentsInChildren<NavMeshObstacle>();
            obs_adj = Room[adjacentRoom].GetComponentsInChildren<NavMeshObstacle>();
            obs_open[2].enabled = false;
            obs_adj[3].enabled = false;
            obs_open = null;
            obs_adj = null;

            deleteform = Room[adjacentRoom].transform.GetChild(2).GetChild(10);
            Room[adjacentRoom].GetComponent<EnemyGroup>().GenPoint.Remove(deleteform);
            deleteform = Room[idx].transform.GetChild(19).GetChild(10);
            Room[idx].GetComponent<EnemyGroup>().GenPoint.Remove(deleteform);
        }
        adjacentRoom = idx - 5;//오른쪽 아래 방
        adj_i = adjacentRoom / MaxSide;
        adj_j = adjacentRoom % MaxSide;
        if (idx >= 5 && adj_i == (i - 1) && adj_j == j && Room[adjacentRoom].gameObject.activeSelf)
        {
            obs_open = Room[idx].GetComponentsInChildren<NavMeshObstacle>();
            obs_adj = Room[adjacentRoom].GetComponentsInChildren<NavMeshObstacle>();
            obs_open[1].enabled = false;
            obs_adj[0].enabled = false;
            obs_open = null;
            obs_adj = null;

            deleteform = Room[adjacentRoom].transform.GetChild(11).GetChild(18);
            Room[adjacentRoom].GetComponent<EnemyGroup>().GenPoint.Remove(deleteform);
            deleteform = Room[idx].transform.GetChild(11).GetChild(1);
            Room[idx].GetComponent<EnemyGroup>().GenPoint.Remove(deleteform);
        }
        adjacentRoom = idx + 5;//왼쪽 위 방
        adj_i = adjacentRoom / MaxSide;
        adj_j = adjacentRoom % MaxSide;
        if (idx < Room.Count - 5 && adj_i == (i + 1) && adj_j == j && Room[adjacentRoom].gameObject.activeSelf)
        {
            obs_open = Room[idx].GetComponentsInChildren<NavMeshObstacle>();
            obs_adj = Room[adjacentRoom].GetComponentsInChildren<NavMeshObstacle>();
            obs_open[0].enabled = false;
            obs_adj[1].enabled = false;
            obs_open = null;
            obs_adj = null;
            
            deleteform = Room[adjacentRoom].transform.GetChild(11).GetChild(1);
            Room[adjacentRoom].GetComponent<EnemyGroup>().GenPoint.Remove(deleteform);
            deleteform = Room[idx].transform.GetChild(11).GetChild(18);
            Room[idx].GetComponent<EnemyGroup>().GenPoint.Remove(deleteform);
        }
    }

    public void OpenRoom(int idx)
    {
        Room[idx].SetActive(true);
        //checkOutside();
        SetObstacles(idx);
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

    public bool isCameraInRoom()
    {
        Vector3 cameraPos = Camera.main.transform.position;
        cameraPos.y = 0;

        cameraPos += (prePos - conPos);
        for (int i = 0; i < Room.Count; i++)
        {
            if (Room[i].activeInHierarchy)
            {
                if ((cameraPos - Room[i].transform.position).magnitude < 10f)
                    return true;
            }
        }

        return false;
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

    public void OnOffHitCollider()
    {
        for (int i = 0; i < Room.Count; i++)
        {
            if (Room[i].activeInHierarchy)
                Room[i].GetComponent<TileManager>().OnOffHitCollider();
        }
    }


    public void ResetRoom()
    {
        for (int i = 0; i < Room.Count; i++)
        {
            Room[i].GetComponent<TileManager>().AllDestroy();
            Room[i].GetComponent<TileManager>().InitTileManager();
        }
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
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                if (isMove && possibleDrag)
                {
                    conPos = GetRay();
                    if (isCameraInRoom())
                        Camera.main.transform.position += (prePos - conPos);
                    prePos = conPos;
                }
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
