using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EXPManager : MonoBehaviour {

    public Image expBar;
    public Text fameText;
    public Text prizeText;
    public int EXP = 60;
    public int ExpRequirementofThisLevel = 70;

    public void AddEXP(int num) // 경험치 추가 함수
    {
        EXP += num; // 경험치 더하기
        if ( EXP >= ExpRequirementofThisLevel )
        {
            //Fame += 1;
            EXP =- ExpRequirementofThisLevel;   
        }
    }

    public void DisplayFame()
    {
        //fameText.text = Fame;
    }

    public void DisplayPrize(int prize)
    {
        prizeText.text = prize + "000";
    }

    public void DisplayEXP()
    {
        expBar.fillAmount = ((float)EXP / (float)ExpRequirementofThisLevel);
    }

    void Start()
    {

    }

    void Update()
    {
        DisplayEXP();
    }
}
