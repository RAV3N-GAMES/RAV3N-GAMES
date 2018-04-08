using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EXPManager : MonoBehaviour {

    public Image expBar;
    public Text fameText;
    public Text prizeText;
    
    public void DisplayExInfo() {
        fameText.text = Data_Player.Fame.ToString();
        prizeText.text = Data_Player.Experience.ToString();
        expBar.fillAmount = ((float)Data_Player.Experience / (float)Data_Player.LvExperience);
    }

    void Awake() {
        Data_Player.Fame = 4;
        Data_Player.Experience = 100;
        prizeText.color = Color.yellow;
        prizeText.fontSize += 3;
    }
    void Start()
    {

    }

    void Update()
    {
        DisplayExInfo();
    }
}
