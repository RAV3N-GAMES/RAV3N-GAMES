using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatePopUp : MonoBehaviour {
    [HideInInspector]
    public GameObject Obj;

    public void CreatePref()
    {
        if (Obj.GetComponent<CheckTile>().isPossible)
        {
            Obj.GetComponent<ObjectInfo>().OnDisplay();
            Obj.GetComponent<DisplayObject>().UsingTile();
        }
        else
            Destroy(Obj);

        gameObject.SetActive(false);
        PossibleDrag();
    }

    public void RotationPref()
    {
        Obj.GetComponent<ObjectInfo>().rotationObject();
        
        Obj.GetComponent<CheckTile>().OnDisplayCheckTile();
    }

    public void CancelPref()
    {
        Destroy(Obj);
        gameObject.SetActive(false);
        PossibleDrag();
    }

    public void PossibleDrag()
    {
        RoomManager.possibleDrag = true;
    }
}
