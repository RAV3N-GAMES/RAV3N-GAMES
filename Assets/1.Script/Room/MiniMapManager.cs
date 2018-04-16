using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniMapManager : MonoBehaviour {
    public GameObject RoomPref;
    public Transform RoomParent;
          
    List<GameObject> RoomList;

    int conRoom;

    const int STEP_MAX = 5;

    void Awake()
    {
        conRoom = 0;
        RoomList = new List<GameObject>();

        SetRoomParentRect();

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
    

    public void MoveMapRoom(int idx)
    {
        if (MapManager.isOpen[idx / STEP_MAX][idx % STEP_MAX] != MapManager.Type.OPEN)
            return;

        int roomIdx = (idx / STEP_MAX) * MapManager.Step + (idx % STEP_MAX);
        RoomList[conRoom].GetComponent<Image>().color = new Color(1, 1, 1, 1);
        RoomList[roomIdx].GetComponent<UnityEngine.UI.Image>().color = new Color(0, 1, 0, 1);

        conRoom = roomIdx;
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
        roomRect.anchorMin = new Vector2(size * (idx % MapManager.Step), size * (idx / MapManager.Step));
        roomRect.anchorMax = new Vector2(size * ((idx % MapManager.Step) + 1), size * ((idx / MapManager.Step) + 1));

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

    int GetOpenRoomCnt()
    {
        int openRoomCnt = 0;
        for (int idx = 0; idx < STEP_MAX * STEP_MAX; idx++)
        {
            if (MapManager.isOpen[idx / STEP_MAX][idx % STEP_MAX] == MapManager.Type.OPEN)
                openRoomCnt++;
        }

        return openRoomCnt;
    }

    public void InitMap()
    {
        DestroyRoomPref();
        int Step = MapManager.Step;
        float size = 1f / (float)Step;

        for (int i = 0; i < Step; i++)
        {
            for (int j = 0; j < Step; j++)
            {
                if (MapManager.isOpen[i][j] == MapManager.Type.DISABLE)
                    break;

                GameObject newRoom = CreateRoomPref((i * STEP_MAX + j).ToString());
                InitRoomRect(newRoom.GetComponent<RectTransform>(), size, i * Step + j);

                if (MapManager.isOpen[i][j] == MapManager.Type.CLOSE){
                    if (GetOpenRoomCnt() >= MapManager.tempFame)
                        newRoom.SetActive(false);
                    else
                        newRoom.GetComponent<Image>().color = new Color(0, 0, 1, 1);
                }
            }
        }

        RoomList[conRoom].GetComponent<Image>().color = new Color(0, 1, 0, 1);
    }
}
