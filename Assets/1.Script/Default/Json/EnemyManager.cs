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
    public static List<Enemy> Tbl_EnemySetup = new List<Enemy>();

    // Use this for initialization
    void Start () {
        ReadMain(Path1);
        ReadMain(Path2);
        ReadMain(Path3);
        ReadMain(Path4);
        ReadMain(Path5);
        ReadMain(Path6);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    public override void ParsingJson(JsonData data)
    {
        for (int i = 0; i < data.Count; i++)
        {
            Enemy tmp = new Enemy();
            tmp.Fame = int.Parse(data[i]["Fame"].ToString());
            tmp.Level = 1;
            tmp.HP= int.Parse(data[i]["HP"].ToString());
            tmp.Attack = int.Parse(data[i]["Attack"].ToString());
            tmp.BuildingAttack= int.Parse(data[i]["BuildingAttack"].ToString());
            tmp.id = data[i]["id"].ToString();
            tmp.Type = int.Parse(data[i]["Type"].ToString());
            Tbl_EnemySetup.Add(tmp);
        }
    }
}
