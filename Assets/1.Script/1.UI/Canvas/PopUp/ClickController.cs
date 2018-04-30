using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickController : MonoBehaviour {

    public GameObject contentsButton;
    public GameObject settingWindow;
    public GameObject On, Off;

    public void CheckClickedState()
    {
        if( contentsButton.activeInHierarchy )
        {
            contentsButton.SetActive(false);
        }

        else
        {
            contentsButton.SetActive(true);
        }
    }

    public void SettingWindowControl()
    {
        if (contentsButton.activeInHierarchy)
        {
            contentsButton.SetActive(false);
            settingWindow.SetActive(false);
        }

        else
        {
            contentsButton.SetActive(true);
            settingWindow.SetActive(true);
        }
    }

    public void OnButtonClick()
    {
        On.SetActive(false);
        Off.SetActive(true);
    }

    public void OffButtonClick()
    {
        On.SetActive(true);
        Off.SetActive(false);
    }
}
