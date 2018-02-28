using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatePopUp : MonoBehaviour {
    [HideInInspector]
    public GameObject Obj;

    public void CreatePref()
    {
        Obj.GetComponent<ObjectInfo>().OnDisplay();
        Obj.GetComponent<DisplayObject>().UsingTile();

        gameObject.SetActive(false);

        PossibleDrag();
    }

    public void RotationPref()
    {
        Obj.GetComponent<ObjectInfo>().rotationObject();
        PossibleDrag();
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
