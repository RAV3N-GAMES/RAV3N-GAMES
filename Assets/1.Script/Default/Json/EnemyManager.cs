using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;

//주의!! Enemy에는 Level이 없음 -> 일단은 1로 고정
public class EnemyManager : JsonReadWrite
{
    /*
     * Tbl_EnemySetup Index
     * 0 ~ 21 : JuvenileDelinquents
     * 22 ~ 43 : MoneyLender
     * 44 ~ 65 : Pickpocket
     * 66 ~ 87 : IEDFarmer
     * 88 ~ 109 : Meateater
     * 110 ~ 131 : WanderingMinstrel
     */

    const string Path1 = "./Assets/Resources/Data/Enemy_JuvenileDelinquents.json";
    const string Path2 = "./Assets/Resources/Data/Enemy_MoneyLender.json";
    const string Path3 = "./Assets/Resources/Data/Enemy_Pickpocket.json";
    const string Path4 = "./Assets/Resources/Data/Enemy_IEDFarmer.json";
    const string Path5 = "./Assets/Resources/Data/Enemy_Meateater.json";
    const string Path6 = "./Assets/Resources/Data/Enemy_WanderingMinstrel.json";
    public static List<EnemyObject> Tbl_EnemySetup = new List<EnemyObject>();

    // Use this for initialization
    void Start () {
        ReadMain(Path1);
        ReadMain(Path2);
        ReadMain(Path3);
        ReadMain(Path4);
        ReadMain(Path5);
        ReadMain(Path6);
        /*입력값 확인용 코드입니다. 지우지 마세요. 
        for (int i = 0; i < 22*6; i++)
        {
            Debug.Log(i + "th index: " + Tbl_EnemySetup[i].Level+ " " + Tbl_EnemySetup[i].Fame+ " " + Tbl_EnemySetup[i].HP+ " " + Tbl_EnemySetup[i].Attack+ " " + Tbl_EnemySetup[i].BuildingAttack+ " " + Tbl_EnemySetup[i].Type+ " " + Tbl_EnemySetup[i].id+ " " + Tbl_EnemySetup[i].AttackRange+ " " + Tbl_EnemySetup[i].AttackAngle+ " " + Tbl_EnemySetup[i].HitConstrain+ " " + Tbl_EnemySetup[i].AttackCool+" " +Tbl_EnemySetup[i].MoveSpeed);
        }
        */
    }

    // Update is called once per frame
    void Update () {
		
	}
    public override void ParsingJson(JsonData data)
    {
        for (int i = 0; i < data.Count; i++)
        {
            EnemyObject tmp = new EnemyObject();
            tmp.Fame = int.Parse(data[i]["Fame"].ToString());
            tmp.Level = 1;
            tmp.HP= int.Parse(data[i]["HP"].ToString());
            tmp.Attack = int.Parse(data[i]["Attack"].ToString());
            tmp.BuildingAttack= int.Parse(data[i]["BuildingAttack"].ToString());
            tmp.id = data[i]["id"].ToString();
            tmp.Type = int.Parse(data[i]["Type"].ToString());
            tmp.AttackRange= double.Parse(data[i]["AttackRange"].ToString());
            tmp.AttackAngle= double.Parse(data[i]["AttackAngle"].ToString());
            tmp.HitConstrain= int.Parse(data[i]["HitConstrain"].ToString());
            tmp.AttackCool= double.Parse(data[i]["AttackCool"].ToString()); 
            tmp.MoveSpeed= double.Parse(data[i]["MoveSpeed"].ToString());
            Tbl_EnemySetup.Add(tmp);
        }
    }
}
