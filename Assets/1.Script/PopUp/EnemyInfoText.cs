using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyInfoText : MonoBehaviour
{
    string id;

    int idxBase;

    public Text HP;
    public Text AttackPoint;
    public Text AttackSpeed;

    void Awake()
    {
        id = gameObject.name;

        switch(id)
        {
            case "JuvenileDelinquents":
                idxBase = -4;
                break;
            case "MoneyLender":
                idxBase = 18;
                break;
            case "Pickpocket":
                idxBase = 40;
                break;
            case "IEDFarmer":
                idxBase = 62;
                break;
            case "Meateater":
                idxBase = 84;
                break;
            case "WanderingMinstrel":
                idxBase = 106;
                break;
        }
    }

    public void SetEnemyInfo(bool isSS)
    {
        EnemyObject enemyObject = EnemyManager.Tbl_EnemySetup[idxBase + Data_Player.Fame];

        if (enemyObject.id.Equals(id))
        {
            if (isSS)
            {
                HP.text = (Mathf.CeilToInt(enemyObject.HP * 0.9f)).ToString();
                AttackPoint.text = (Mathf.CeilToInt(enemyObject.Attack * 0.9f)).ToString();
                AttackSpeed.text = ((float)(enemyObject.AttackPeriod * 0.9f)).ToString();
            }
            else
            {
                HP.text = (Mathf.CeilToInt(enemyObject.HP)).ToString();
                AttackPoint.text = (Mathf.CeilToInt(enemyObject.Attack)).ToString();
                AttackSpeed.text = (((float)enemyObject.AttackPeriod)).ToString();
            }
        }
        else
            print("enemeyInfo 잘못된 범위");
    }
}
