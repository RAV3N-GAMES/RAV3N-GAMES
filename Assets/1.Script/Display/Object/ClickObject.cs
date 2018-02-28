using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickObject : MonoBehaviour
{
    float clickTime;
    ObjectInfo objectInfo;

    [HideInInspector]
    public GameObject ChangePopUp;

    void Awake()
    {
        clickTime = 1.5f;
        objectInfo = GetComponent<ObjectInfo>();
    }

    void LongClick()
    {
        if (objectInfo.isDisplay)
        {
            RoomManager.possibleDrag = false;
            ChangePopUp.SetActive(true);
            ChangePopUp.GetComponent<ChangePopUp>().Obj = gameObject;
        }
    }

    public void OnMouseDown()
    {
        Invoke("LongClick", clickTime);
    }

    public void OnMouseUp()
    {
        CancelInvoke("LongClick");
    }
}
