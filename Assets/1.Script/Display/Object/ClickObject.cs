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
        print("longClick");
        if (objectInfo.isDisplay)
        {
            print("last longclick");
            RoomManager.possibleDrag = false;

            ChangePopUp.SetActive(true);
            ChangePopUp.GetComponent<ChangePopUp>().Obj = gameObject;
            ChangePopUp.GetComponent<ChangePopUp>().InitPopUp();
        }
    }

    public void OnMouseDown()
    {
        print("mouseDown");
        Invoke("LongClick", clickTime);
    }

    public void OnMouseUp()
    {
        print("mouseUp");
        CancelInvoke("LongClick");
    }
}
