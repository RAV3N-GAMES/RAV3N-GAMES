using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour {
    public List<GameObject> Room;

    void MoveRoom(int idx)
    {//드래그를 통해서 움직일 경우 해야함
        Camera.main.transform.position = new Vector3(Room[idx].transform.position.x, Room[idx].transform.position.y, Camera.main.transform.position.z);
    }

    void AddRoom(int idx)
    {
        Room[idx].SetActive(true);
    }
}
