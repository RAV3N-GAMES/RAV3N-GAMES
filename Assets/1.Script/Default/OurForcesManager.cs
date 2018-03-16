using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;


/*
 * Tbl_OurForceSetup Index
 * 0 ~ 99 : Gaurd
 * 100 ~ 199 : QuickReactionForce
 * 200 ~ 299 : BiochemistryUnit
 * 300 ~ 399 : Researcher
 */
 
public class OurForcesManager : JsonReadWrite {
    const string Path1 = "./Assets/Resources/Data/OurForces_Guard.json";
    const string Path2 = "./Assets/Resources/Data/OurForces_QuickReactionForce.json";
    const string Path3 = "./Assets/Resources/Data/OurForces_BiochemistryUnit.json";
    const string Path4 = "./Assets/Resources/Data/OurForces_Researcher.json";
    public static List<OurForcesObject> Tbl_OurForceSetup = new List<OurForcesObject>();

    // Use this for initialization
    void Start () {
        ReadMain(Path1);
        ReadMain(Path2);
        ReadMain(Path3);
        ReadMain(Path4);
        /* 입력값 확인용 코드입니다. 지우지 마세요.       
                for (int i = 0; i < 400; i++)
                {
                    Debug.Log(i + "th index: " + Tbl_OurForceSetup[i].Level+" " + Tbl_OurForceSetup[i].HP + " " + Tbl_OurForceSetup[i].Attack + " " + Tbl_OurForceSetup[i].SkillCool + " " + Tbl_OurForceSetup[i].Price + " " + Tbl_OurForceSetup[i].UpgradeCost+ " " + Tbl_OurForceSetup[i].HealCost + " " + Tbl_OurForceSetup[i].id + " " + Tbl_OurForceSetup[i].Type + " " + Tbl_OurForceSetup[i].ActiveCost + " " + Tbl_OurForceSetup[i].AttackRange + " " + Tbl_OurForceSetup[i].AttackAngle + " " + Tbl_OurForceSetup[i].HitConstrain+ " " + Tbl_OurForceSetup[i].AttackCool+ " " + Tbl_OurForceSetup[i].EnemyRecognizeRangeHalf + " " + Tbl_OurForceSetup[i].MoveSpeed + " ");
                }
        */
    }

    // Update is called once per frame
    void Update () {
		
	}
    public override void ParsingJson(JsonData data)
    {
        for (int i = 0; i < data.Count; i++) {
            OurForcesObject tmp = new OurForcesObject();
            tmp.Level = int.Parse(data[i]["Level"].ToString());
            tmp.HP = int.Parse(data[i]["HP"].ToString());
            tmp.Attack= int.Parse(data[i]["Attack"].ToString());
            tmp.SkillCool= int.Parse(data[i]["SkillCool"].ToString());
            tmp.Price = int.Parse(data[i]["Price"].ToString());
            tmp.UpgradeCost = int.Parse(data[i]["UpgradeCost"].ToString());
            tmp.HealCost= double.Parse(data[i]["HealCost"].ToString());
            tmp.id = data[i]["id"].ToString();
            tmp.Type = int.Parse(data[i]["Type"].ToString());
            tmp.ActiveCost= int.Parse(data[i]["ActiveCost"].ToString());
            tmp.AttackRange= double.Parse(data[i]["AttackRange"].ToString());
            tmp.AttackAngle = double.Parse(data[i]["AttackAngle"].ToString());
            tmp.HitConstrain = int.Parse(data[i]["HitConstrain"].ToString());
            tmp.AttackCool = double.Parse(data[i]["AttackCool"].ToString());
            tmp.EnemyRecognizeRangeHalf = double.Parse(data[i]["EnemyRecognizeRangeHalf"].ToString());
            tmp.MoveSpeed= double.Parse(data[i]["MoveSpeed"].ToString());
            Tbl_OurForceSetup.Add(tmp);
        }
    }
}
