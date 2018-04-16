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
        Data_Player.Fame = 10;
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

    public static void addExp(int exp) {
        Data_Player.addExperience(exp);
        if (Data_Player.Experience >= Data_Player.LvExperience) {
            LvUp();
        }
        
    }

    private static void LvUp() {
        Data_Player.Fame++;
        foreach (SecretActs s in SecretManager.SecretList) {
            s.CheckChange();
        }
    }
}
