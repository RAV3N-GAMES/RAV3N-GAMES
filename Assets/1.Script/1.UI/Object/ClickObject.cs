using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickObject : MonoBehaviour
{
    public static bool isPossibleClick = false;

    float clickTime;
    [HideInInspector]
    public ObjectInfo objectInfo;

    [HideInInspector]
    public GameObject ChangePopUpManager;

    public static TaskObject taskObject;

    void Awake()
    {
        clickTime = 1f;

        objectInfo = GetComponent<ObjectInfo>();
    }

    void LongClick()
    {
        if (objectInfo.isDisplay && isPossibleClick)
        {
            if (!DayandNight.isDay) {
                RoomManager.ChangeClickStatus(false);
                SoundManager.soundManager.OnEffectSound("8_CONTENTS");

                ChangePopUpManager.transform.parent.gameObject.SetActive(true);
                ChangePopUpManager.GetComponent<ChangePopUpManager>().InitChangePopUp(gameObject, GetComponent<ObjectInfo>().type);

                GetComponent<ObjectColor>().OnRecognizeRage(true);

                if (taskObject != null)
                    taskObject.OnLongSelect();
            }
        }
    }

    public void OnMouseDown()
    {
        if (isPossibleClick)
        {
            if (!RoomManager.isTransparency || GetComponent<ObjectInfo>().type != 0)
            {
                Invoke("LongClick", clickTime);
            }
        }
    }

    public void OnMouseUp()
    {
        CancelInvoke("LongClick");
    }
}
