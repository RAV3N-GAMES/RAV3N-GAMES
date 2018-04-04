using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;

/*
 * Tbl_BuildingSetup Index
 * 0 ~ 99 : OldBuilding
 * 100 ~ 199 : NewBuilding
 * 200 ~ 299 : Functional Building
 * 300 ~ 399 : CoreBuilding
 */
public class BuildingManager : JsonReadWrite
{
    const string Path1 = "./Assets/Resources/Data/Building_Old.json";
    const string Path2 = "./Assets/Resources/Data/Building_New.json";
    const string Path3 = "./Assets/Resources/Data/Building_Functional.json";
    const string Path4 = "./Assets/Resources/Data/Building_Core.json";
    public static List<BuildingObject> Tbl_BuildingSetup = new List<BuildingObject>();

    // Use this for initialization
    void Awake () {
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
        for (int i = 0; i < data.Count; i++)
        {
            BuildingObject tmp = new BuildingObject();
            tmp.Level = int.Parse(data[i]["Level"].ToString());
            tmp.Type = int.Parse(data[i]["Type"].ToString());
            tmp.id = data[i]["id"].ToString();
            tmp.HP = int.Parse(data[i]["HP"].ToString());
            tmp.Price = int.Parse(data[i]["Price"].ToString());
            tmp.UpgradeCost = int.Parse(data[i]["UpgradeCost"].ToString());
            tmp.RepairCost = int.Parse(data[i]["RepairCost"].ToString());
            tmp.ActiveCost = int.Parse(data[i]["ActiveCost"].ToString());
            Tbl_BuildingSetup.Add(tmp);
        }
    }
}
