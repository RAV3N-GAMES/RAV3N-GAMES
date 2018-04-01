using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickController : MonoBehaviour {

    public GameObject On, Off;
    public GameObject Menu;
    public GameObject ClickBlockPanelMenu;

    // 설정창 On 버튼 클릭 시
    public void OnButtonClick()
    {
        On.SetActive(false);
        Off.SetActive(true);
    }

    // 설정창 Off 버튼 클릭 시시
    public void OffButtonClick()
    {
        On.SetActive(true);
        Off.SetActive(false);
    }

    public void MenuOnOff()
    {
        if (Menu.activeInHierarchy)
        {
            Menu.SetActive(false);
            ClickBlockPanelMenu.SetActive(false);
        }

        else
        {
            Menu.SetActive(true);
            ClickBlockPanelMenu.SetActive(true);
        }
    }

}
