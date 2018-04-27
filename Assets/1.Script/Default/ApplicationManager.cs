using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplicationManager : MonoBehaviour {
    public GameObject QuitPopUp;

    void Update()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                if (QuitPopUp == null)
                    Application.Quit();
                else
                    QuitPopUp.SetActive(!QuitPopUp.activeInHierarchy);
            }
        }
    }

    public void ApplicationQuit()
    {
        Application.Quit();
    }
}
