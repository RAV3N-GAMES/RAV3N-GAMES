using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyInfoPopUp : MonoBehaviour {
    public GameObject ActiveSSButton;
    public GameObject ActivePHButton;

    public Text chanceText;

    public List<EnemyInfoText> enemyInfoText;

    bool isSS = false;

    void Start()
    {
        ChangeStatus(true);
    }

    public void ChangeChanceText()
    {
        for (int i = 0; i < enemyInfoText.Count; i++)
            enemyInfoText[i].SetEnemyInfo(isSS);
    }

    public void ChangeStatus(bool isSS)
    {
        if (this.isSS == isSS)
            return;

        this.isSS = isSS;

        ActiveSSButton.SetActive(isSS);
        ActivePHButton.SetActive(!isSS);

        ChangeChanceText();
    }
}
