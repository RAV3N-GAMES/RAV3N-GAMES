using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour {
    public Camera minimapCamera;
    public Transform RoomParent;
    List<GameObject> Room;

    [HideInInspector]
    public int CenterRoomIdx;

    [HideInInspector]
    public static bool possibleDrag;
    bool isMove;

    Vector3 prePos;
    Vector3 conPos;

    void Awake()
    {
        possibleDrag = true;
        isMove = false;
        CenterRoomIdx = 0;

        Room = new List<GameObject>();

        for(int i = 0; i < RoomParent.childCount; i++)
        {
            Room.Add(RoomParent.GetChild(i).gameObject);
        }
    }

    public void MoveRoom(int idx){
        Camera.main.transform.position = new Vector3(Room[idx].transform.position.x, Camera.main.transform.position.y, Room[idx].transform.position.z);
        minimapCamera.transform.position= new Vector3(Room[idx].transform.position.x, minimapCamera.transform.position.y, Room[idx].transform.position.z);
    }

    public void OpenRoom(int idx)
    {
        Room[idx].SetActive(true);
    }

    Vector3 GetRay()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 pos = ray.direction;
        pos.y = 0;

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
        }

        if(isMove && possibleDrag)
        {
            conPos = GetRay();

            Camera.main.transform.position += (prePos - conPos) * 10f;
            prePos = conPos;
        }
    }
}
