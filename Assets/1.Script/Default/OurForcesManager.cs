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
    public static List<OurForces> Tbl_OurForceSetup = new List<OurForces>();

    // Use this for initialization
    void Start () {
        ReadMain(Path1);
        ReadMain(Path2);
        ReadMain(Path3);
        ReadMain(Path4);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    public override void ParsingJson(JsonData data)
    {
        for (int i = 0; i < data.Count; i++) { 
            OurForces tmp = new OurForces();
            tmp.Level = int.Parse(data[i]["Level"].ToString());
            tmp.HP = int.Parse(data[i]["HP"].ToString());
            tmp.Attack= int.Parse(data[i]["Attack"].ToString());
            tmp.SkillCool= int.Parse(data[i]["SkillCool"].ToString());
            tmp.Price = int.Parse(data[i]["Price"].ToString());
            tmp.UpgradeCost = int.Parse(data[i]["UpgradeCost"].ToString());
            tmp.HealCost= double.Parse(data[i]["HealCost"].ToString());
            tmp.id = data[i]["id"].ToString();
            tmp.Type = int.Parse(data[i]["Type"].ToString());

            Tbl_OurForceSetup.Add(tmp);
        }
    }
}
