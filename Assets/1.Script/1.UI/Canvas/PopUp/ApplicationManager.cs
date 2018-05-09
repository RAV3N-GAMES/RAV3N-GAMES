using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplicationManager : MonoBehaviour {
    public GameObject QuitPopUp;

    public GameObject Option;

    public Transform PopUp;
    public Transform Warning;

    public GameObject Map;

    List<GameObject> PopUpList;

    public static bool isPossible = true;

    void Awake()
    {
        if (QuitPopUp != null)
        {
            PopUpList = new List<GameObject>();

            PopUpList.Add(QuitPopUp);

            PopUpList.Add(Option);

            int WarningCnt = Warning.childCount;
            for (int i = 0; i < WarningCnt; i++)
            {
                PopUpList.Add(Warning.GetChild(WarningCnt - 1 - i).gameObject);
            }

            int PopUpCnt = PopUp.childCount;
            for (int i = 0; i < PopUpCnt; i++)
                PopUpList.Add(PopUp.GetChild(PopUpCnt - 1 - i).gameObject);

            PopUpList.Add(Map);
        }
    }

    void Update()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                if (QuitPopUp == null)
                    Application.Quit();
                else
                {
                    if (!ClosePopUp())
                        QuitPopUp.SetActive(!QuitPopUp.activeInHierarchy);
                }
            }
        }
    }

    bool ClosePopUp()
    {
        if (!isPossible)
            return true;
        for(int i = 0; i < PopUpList.Count; i++)
        {
            if(PopUpList[i].activeInHierarchy)
            {
                PopUpList[i].SetActive(false);
                RoomManager.ChangeClickStatus(true);

                return true;
            }
        }

        return false;
    }

    public void ApplicationQuit()
    {
        Application.Quit();
    }
}
