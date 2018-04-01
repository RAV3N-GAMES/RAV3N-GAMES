using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapManager : MonoBehaviour {
    public Transform MapCamera;

    public GameObject RoomPref;
    public Transform RoomParent;
    public RectTransform ObjPos;
    public RoomManager roomManager;
    //public GameObject ObjectPosPref;

    public MiniMapManager miniMapManager;

    static int Step;           //이거 저장해놨다가 불러야함 //Step * Step 개의 방 사용
    List<GameObject> RoomList; //접근하는 인덱스 바꿔야함
    Type[][] isOpen;

    //List<GameObject> ObjPosList;

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

        RoomList = new List<GameObject>();
        //ObjPosList = new List<GameObject>();

        SetRoomParentRect();



        isOpen = new Type[STEP_MAX][];
        for(int i = 0; i < STEP_MAX; i++)
        {
            isOpen[i] = new Type[STEP_MAX];
        }

        InitRoomStatus(Type.DISABLE, STEP_MAX);
        InitRoomStatus(Type.OPEN, Step);

        InitMap();
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

    bool isAllOpen()
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

    public void StepUp()
    {
        if (Step == STEP_MAX)
            return;
        if (!isAllOpen())
            return;

        Step++;
        conRoom = (conRoom / (Step -1)) * Step + (conRoom % (Step-1));

        for (int i = 0; i < Step - 1; i++)
        {
            isOpen[i][Step - 1] = Type.CLOSE;
        }

        for (int i = 0; i < Step - 1; i++)
        {
            isOpen[Step - 1][i] = Type.CLOSE;
        }

        float z = (roomManager.Room[((Step - 1) * STEP_MAX + (Step - 1))].transform.position.z - roomManager.Room[0].transform.position.z) / 2f;

        MapCamera.transform.position = new Vector3(0, 10, z);

        //print("z : " + 0 + "," + ((Step - 1) * STEP_MAX + (Step - 1)) + " pos : " + z);
        MapCamera.gameObject.GetComponent<Camera>().orthographicSize = Step * 16;

        miniMapManager.StepUp();
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

    public void OpenRoom(int idx)
    {
        int i = idx / STEP_MAX;
        int j = idx % STEP_MAX;

        isOpen[i][j] = Type.OPEN;
        RoomList[(idx / STEP_MAX) * Step + (idx % STEP_MAX)].GetComponent<Image>().color = new Color(255, 255, 255, 255);
        
        if(isAllOpen())
        {
            isOpen[Step - 1][Step - 1] = Type.OPEN;
            roomManager.OpenRoom((Step - 1) * (STEP_MAX + 1));
            InitMap();
        }
        
        roomManager.OpenRoom(idx);
        miniMapManager.OpenRoom(idx);
    }

    //void DestroyObjPosPref()
    //{
    //    for(int i = 0; i < ObjPosList.Count; i++)
    //    {
    //        Destroy(ObjPosList[i]);
    //    }
    //
    //    ObjPosList.Clear();
    //}

    //public void InitObjectPos()
    //{
    //    if (ObjectPosPref == null) //수정해야함
    //        return;
    //
    //    DestroyObjPosPref();
    //
    //    for (int i = 0; i < RoomList.Count; i++)
    //    {
    //        int idx = (i % Step) + (i / Step) * STEP_MAX;
    //        TileManager tileManager = roomManager.Room[idx].GetComponent<TileManager>();
    //        int ObjCnt = tileManager.GetObjectCount();
    //
    //        for (int j = 0; j < ObjCnt; j++)
    //        {
    //            GameObject ObjPos = Instantiate(ObjectPosPref, RoomList[i].transform);
    //            RectTransform posRect = ObjPos.GetComponent<RectTransform>();
    //
    //            float[] objInfo = tileManager.GetObjectInfo(j);
    //
    //            posRect.anchorMin = new Vector2((float)objInfo[0] / 20f, (float)objInfo[1] / 20f);
    //            posRect.anchorMax = new Vector2((float)objInfo[0] / 20f, (float)objInfo[1] / 20f);
    //            
    //            posRect.offsetMin = Vector2.zero;
    //            posRect.offsetMax = Vector2.zero;
    //
    //            posRect.sizeDelta = new Vector2(50, 50);
    //            
    //            switch ((int)objInfo[2])
    //            {
    //                case 0: ObjPos.GetComponent<Image>().color = new Color(0.3f, 0, 0); break;
    //                case 1: ObjPos.GetComponent<Image>().color = new Color(1, 1, 0); break;
    //                case 2: ObjPos.GetComponent<Image>().color = new Color(0, 0, 1); break;
    //                case 3: ObjPos.GetComponent<Image>().color = new Color(1, 0, 1); break;
    //                case 4: ObjPos.GetComponent<Image>().color = new Color(0, 1, 1); break;
    //                case 5: ObjPos.GetComponent<Image>().color = new Color(0, 1, 0); break;
    //                default: break;
    //            }
    //            ObjPosList.Add(ObjPos);
    //        }
    //    }
    //}

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
                    newRoom.GetComponent<UnityEngine.UI.Image>().color = new Color(0, 0, 1, 1);
            }
        }

        RoomList[conRoom].GetComponent<UnityEngine.UI.Image>().color = new Color(0, 1, 0, 1);

        //InitObjectPos();
    }

}
