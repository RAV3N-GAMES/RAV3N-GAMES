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
        clickTime = 1f;
        isPossibleClick = false;

        objectInfo = GetComponent<ObjectInfo>();
    }

    void LongClick()
    {
        if (objectInfo.isDisplay)
        {
            if (!DayandNight.isDay) {
                RoomManager.ChangeClickStatus(false);

                ChangePopUp.SetActive(true);
                ChangePopUp.GetComponent<ChangePopUp>().Obj = gameObject;
                ChangePopUp.GetComponent<ChangePopUp>().InitPopUp();

                GetComponent<ObjectColor>().OnRecognizeRage(true);
            }
        }
    }

    public void OnMouseDown()
    {
        if (isPossibleClick)
        {
            Invoke("LongClick", clickTime);
        }
    }

    public void OnMouseUp()
    {
        CancelInvoke("LongClick");
    }
}
