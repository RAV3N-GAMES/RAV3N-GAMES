using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniMapManager : MonoBehaviour {
    public GameObject RoomPref;
    public Transform RoomParent;

    static int Step;           
    List<GameObject> RoomList;
    Type[][] isOpen;

    public enum Type
    {
        OPEN,
        CLOSE,
        DISABLE
    }

    int conRoom;

    const int STEP_MAX = 5;

    void Awake()
    {
        Step = 2;
        conRoom = 0;
        RoomList = new List<GameObject>();

        SetRoomParentRect();

        isOpen = new Type[STEP_MAX][];

        for (int i = 0; i < STEP_MAX; i++)
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
        float rectSize = Mathf.Sqrt(1 / 2f) * rectY;

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
        for (int i = 0; i < Step - 1; i++)
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
        conRoom = (conRoom / (Step - 1)) * Step + (conRoom % (Step - 1));

        for (int i = 0; i < Step - 1; i++)
        {
            isOpen[i][Step - 1] = Type.CLOSE;
        }

        for (int i = 0; i < Step - 1; i++)
        {
            isOpen[Step - 1][i] = Type.CLOSE;
        }

        InitMap();
    }

    public void MoveMapRoom(int idx)
    {
        if (isOpen[idx / STEP_MAX][idx % STEP_MAX] != Type.OPEN)
            return;

        int roomIdx = (idx / STEP_MAX) * Step + (idx % STEP_MAX);
        RoomList[conRoom].GetComponent<Image>().color = new Color(1, 1, 1, 1);
        RoomList[roomIdx].GetComponent<UnityEngine.UI.Image>().color = new Color(0, 1, 0, 1);

        conRoom = roomIdx;
    }

    public void OpenRoom(int idx)
    {
        int i = idx / STEP_MAX;
        int j = idx % STEP_MAX;

        isOpen[i][j] = Type.OPEN;
        RoomList[(idx / STEP_MAX) * Step + (idx % STEP_MAX)].GetComponent<Image>().color = new Color(255, 255, 255, 255);

        if (isAllOpen())
        {
            isOpen[Step - 1][Step - 1] = Type.OPEN;
            InitMap();
        }
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
        RoomList.Add(newRoom);

        newRoom.GetComponent<Button>().enabled = false;

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
    }
}
