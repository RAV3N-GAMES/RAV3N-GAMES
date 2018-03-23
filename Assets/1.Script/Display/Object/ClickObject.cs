using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickObject : MonoBehaviour
{
    public static bool isPossibleClick;

    float clickTime;
    [HideInInspector]
    public ObjectInfo objectInfo;

    [HideInInspector]
    public GameObject ChangePopUp;

    void Awake()
    {
        clickTime = 1.5f;
        isPossibleClick = true;

        objectInfo = GetComponent<ObjectInfo>();
    }

    void LongClick()
    {
        if (objectInfo.isDisplay)
        {
            RoomManager.possibleDrag = false;

            ChangePopUp.SetActive(true);
            ChangePopUp.GetComponent<ChangePopUp>().Obj = gameObject;
            ChangePopUp.GetComponent<ChangePopUp>().InitPopUp();

            isPossibleClick = false;
        }
    }

    public void OnMouseDown()
    {
        if (isPossibleClick)
            Invoke("LongClick", clickTime);
    }

    public void OnMouseUp()
    {
        CancelInvoke("LongClick");
    }
}
