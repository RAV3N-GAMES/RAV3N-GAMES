using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapRoom : MonoBehaviour {
    [HideInInspector]
    public MapManager mapManager;

	public void ClickRoom()
    {
        int roomNum = System.Int32.Parse(name);
        if (mapManager.GetIsOpen(roomNum) == MapManager.Type.CLOSE)
        {
            mapManager.OpenRoom(roomNum);
        }
        else
        {
            mapManager.MoveRoom(roomNum);
        }

        SoundManager.soundManager.OnEffectSound("8_CONTENTS");
    }
}
