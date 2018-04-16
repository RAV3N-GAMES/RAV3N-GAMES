﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapManager : MonoBehaviour {
    public Transform MapCameraPos;
    public Camera MapCamera;

    public GameObject RoomPref;
    public Transform RoomParent;
    public RectTransform ObjPos;
    public RoomManager roomManager;

    public MiniMapManager miniMapManager;

    //index : Step..? , value : 활성화되는 Fame
    static int[] Fame = { 0, 0, 0, 0, 0, 2, 3, 4, 5, 6, 7, 8 };

    public static int tempFame = 0;
    public Text tempFameText;

    static float Edge;
    public static int Step;           //이거 저장해놨다가 불러야함 //Step * Step 개의 방 사용
    List<GameObject> RoomList; //접근하는 인덱스 바꿔야함
    public static Type[][] isOpen;

    public Text BuildingText;
    public Text OurForcesText;
    public Text TrapText;
    public Text SecretText;

    public Text AllRepairCost;

    public GameObject LackOfCoin;

    public GameObject AllRepairButton;
    public GameObject ActivedDamageButton;

    int BuildingCnt;
    int OurForcesCnt;
    int TrapCnt;
    int SecretCnt;

    public static int DamageBuildingCnt = 0;
    public static int DamageOurForcesCnt = 0;

    public enum Type
    {
        OPEN,
        CLOSE,
        DISABLE
    }

    int conRoom;

    const int STEP_MAX = 5;
    
	void Awake () {
        Step = 2; 
        conRoom = 0;
        Edge = 0.5f;

        RoomList = new List<GameObject>();

        SetRoomParentRect();

        isOpen = new Type[STEP_MAX][];
        for(int i = 0; i < STEP_MAX; i++)
        {
            isOpen[i] = new Type[STEP_MAX];
        }

        InitRoomStatus(Type.DISABLE, STEP_MAX);
        InitRoomStatus(Type.OPEN, Step);
	}

    void Start()
    {
        InitMap();
        gameObject.SetActive(false);
    }

    public void SetObjectCnt(int type, int Cnt)
    {
        switch (type)
        {
            case 0: BuildingCnt += Cnt; BuildingText.text = BuildingCnt.ToString(); break;
            case 2: OurForcesCnt += Cnt; OurForcesText.text = OurForcesCnt.ToString(); break;
            case 3: TrapCnt += Cnt; TrapText.text = TrapCnt.ToString(); break;
            case 4: SecretCnt += Cnt; SecretText.text = SecretCnt.ToString(); break;
        }
    }

    public void ChangeToNonActiveDamage()
    {
        AllRepairButton.SetActive(true);
        ChangeMapStatus();
    }

    public void ChangeMapStatus()
    {
        bool isActiveAllRepair = AllRepairButton.activeInHierarchy;

        AllRepairButton.SetActive(!isActiveAllRepair);
        ActivedDamageButton.SetActive(!isActiveAllRepair);

        TrapText.transform.parent.gameObject.SetActive(isActiveAllRepair);
        SecretText.transform.parent.gameObject.SetActive(isActiveAllRepair);

        if (isActiveAllRepair)
        {
            MapCamera.cullingMask = LayerMask.GetMask("ObjPos");

            BuildingText.text = BuildingCnt.ToString();
            OurForcesText.text = OurForcesCnt.ToString();
        }
        else
        {
            MapCamera.cullingMask = LayerMask.GetMask("DamageObjPos");

            MapManager.DamageBuildingCnt = 0;
            MapManager.DamageOurForcesCnt = 0;

            SetAllRepairCost();

            BuildingText.text = MapManager.DamageBuildingCnt.ToString();
            OurForcesText.text = MapManager.DamageOurForcesCnt.ToString();
        }
    }

    void SetAllRepairCost()
    {
        AllRepairCost.text = roomManager.GetAllRepairCost().ToString() + "/" + Data_Player.Gold;
    }

    public void AllRepair()
    {
        if (!roomManager.AllRepair())
        {
            LackOfCoin.SetActive(true);
        }
        else
            RoomManager.ChangeClickStatus(true);
    }

    void SetRoomParentRect()
    {
        RectTransform rect = GetComponent<RectTransform>();
        float anchorY = rect.anchorMax.y - rect.anchorMin.y;

        float rectY = GameObject.Find("Canvas").GetComponent<RectTransform>().sizeDelta.y * anchorY * 0.9f;

        ObjPos.sizeDelta = new Vector2(rectY, rectY * 2);

        float rectSize = Mathf.Sqrt(1/2f) * rectY;

        RoomParent.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(rectSize, rectSize);
    }

    void InitRoomStatus(Type type, int Cnt)
    {
        for (int i = 0; i < Cnt; i++)
        {
            for (int j = 0; j < Cnt; j++)
            {
                isOpen[i][j] = type;
            }
        }
    }

    public Type GetIsOpen(int idx)
    {
        int i = idx / STEP_MAX;
        int j = idx % STEP_MAX;

        return isOpen[i][j];
    }

    bool isEdgeOpen()
    {
        if (isOpen[Step - 1][Step - 1] == Type.OPEN)
            return true;
        return false;
    }

    bool isAllSideOpen()
    {
        for (int i = 0; i < Step - 1 ; i++)
        {
            if (isOpen[i][Step - 1] != Type.OPEN)
            {
                return false;
            }
        }

        for (int i = 0; i < Step - 1; i++)
        {
            if (isOpen[Step - 1][i] != Type.OPEN)
            {
                return false;
            }
        }

        return true;
    }

    public void tempFameUp()
    {
        tempFame++;
        tempFameText.text = tempFame.ToString();

        ChangedFame();
    }

    public void ChangedFame()
    {
        StepUp();
        InitMap();
    }

    public void tempFameDown()
    {
        tempFame--;
        tempFameText.text = tempFame.ToString();

        ChangedFame();
    }

    public void StepUp()
    {
        if (Edge == 0.5f)
        {
            if (Step * Step >= tempFame)
                return;
            if (Step == STEP_MAX)
                return;
        }
        else if(Step * Step > tempFame)
        {
            return;
        }
        
        if (Edge == 0.5f)
        {
            if (!isEdgeOpen())
                return;
        }
        else if (!isAllSideOpen())
            return;
    
        Edge = (Edge + 0.5f) % 1f;
        if (Edge == 0)
            Step++;
    
        if (Edge == 0.5f)
            EdgeOpen();
        else
            NextSideOpen();
    }

    public void EdgeOpen()
    {
        isOpen[Step - 1][Step - 1] = Type.CLOSE;
        
        InitMap();
    }

    public void NextSideOpen()
    {
        conRoom = (conRoom / (Step - 1)) * Step + (conRoom % (Step - 1));

        for (int i = 0; i < Step - 1; i++)
        {
            isOpen[i][Step - 1] = Type.CLOSE;
        }

        for (int i = 0; i < Step - 1; i++)
        {
            isOpen[Step - 1][i] = Type.CLOSE;
        }

        float z = (roomManager.Room[((Step - 1) * STEP_MAX + (Step - 1))].transform.position.z - roomManager.Room[0].transform.position.z) / 2f;

        MapCameraPos.transform.position = new Vector3(0, 10, z);
        MapCameraPos.gameObject.GetComponent<Camera>().orthographicSize = Step * 16;
        
        InitMap();
    }

    public void MoveMapRoom(int idx)
    {
        if (idx == -1)
            idx = roomManager.CenterRoomIdx;
        if (isOpen[idx / STEP_MAX][idx % STEP_MAX] != Type.OPEN)
            return;

        int roomIdx = (idx / STEP_MAX) * Step + (idx % STEP_MAX);
        RoomList[conRoom].GetComponent<Image>().color = new Color(1, 1, 1, 1);
        RoomList[roomIdx].GetComponent<UnityEngine.UI.Image>().color = new Color(0, 1, 0, 1);

        conRoom = roomIdx;
    }
    

    public void MoveRoom(int idx)
    {
        MoveMapRoom(idx);   
        roomManager.MoveRoom(idx);
    }

    int GetOpenRoomCnt()
    {
        int openRoomCnt = 0;
        for (int room = 0; room < STEP_MAX * STEP_MAX; room++)
        {
            if (GetIsOpen(room) == Type.OPEN)
                openRoomCnt++;
        }

        return openRoomCnt;
    }

    public void OpenRoom(int idx)
    {
        if (GetOpenRoomCnt() >= tempFame)
            return;

        int i = idx / STEP_MAX;
        int j = idx % STEP_MAX;

        isOpen[i][j] = Type.OPEN;
        RoomList[(idx / STEP_MAX) * Step + (idx % STEP_MAX)].GetComponent<Image>().color = new Color(255, 255, 255, 255);

        if (Edge == 0.5f)
        {
            if (isEdgeOpen())
                StepUp();
        }
        else if (isAllSideOpen())
            StepUp();
        
        roomManager.OpenRoom(idx);
        InitMap();
    }
    

    void DestroyRoomPref()
    {
        for (int i = 0; i < RoomList.Count; i++)
        {
            Destroy(RoomList[i]);
        }
        RoomList.Clear();
    }

    void InitRoomRect(RectTransform roomRect, float size, int idx)
    {
        roomRect.anchorMin = new Vector2(size * (idx % Step), size * (idx / Step));
        roomRect.anchorMax = new Vector2(size * ((idx % Step) + 1), size * ((idx / Step) + 1));

        roomRect.offsetMin = Vector2.zero;
        roomRect.offsetMax = Vector2.zero;

        roomRect.localRotation = new Quaternion(0, 0, 0, 0);
    }

    GameObject CreateRoomPref(string roomName)
    {
        GameObject newRoom = Instantiate(RoomPref, RoomParent);
        newRoom.name = roomName;
        newRoom.GetComponent<MapRoom>().mapManager = this;
        RoomList.Add(newRoom);

        return newRoom;
    }

    void InitMap() //맵 창에 보이게 하기위함...실제 방은 미리 다 만들어놓고 활성화 여부를 변경해서 사용할 수 있도록 할것
    {
        DestroyRoomPref();
        float size = 1f / (float)Step;

        for (int i = 0; i < Step; i++)
        {
            for (int j = 0; j < Step; j++)
            {
                if (isOpen[i][j] == Type.DISABLE)
                    break;

                GameObject newRoom = CreateRoomPref((i * STEP_MAX + j).ToString());
                InitRoomRect(newRoom.GetComponent<RectTransform>(), size, i * Step + j);

                if (isOpen[i][j] == Type.CLOSE)
                {
                    if (GetOpenRoomCnt() >= tempFame)
                        newRoom.SetActive(false);
                    else
                        newRoom.GetComponent<Image>().color = new Color(0, 0, 1, 1);
                }
            }
        }

        RoomList[conRoom].GetComponent<Image>().color = new Color(0, 1, 0, 1);
        miniMapManager.InitMap();
    }

    
}
