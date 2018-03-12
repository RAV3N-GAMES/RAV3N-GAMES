using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;

/*
     * Tbl_EnemySetup Index
     * 0 ~ 99 : HumanTrap
     * 100~ 199: Warp
     * 200 ~ 299 : FlameThrowingTrap
     * 300 ~ 399 : ObstructMovementCurrent
     */
public class TrapManager : JsonReadWrite
{
    const string Path1 = "./Assets/Resources/Data/Trap_HumanTrap.json";
    const string Path2 = "./Assets/Resources/Data/Trap_Warp.json";
    const string Path3 = "./Assets/Resources/Data/Trap_FlameThrowingTrap.json";
    const string Path4 = "./Assets/Resources/Data/Trap_ObstructMovementCurrent.json";
    public static List<Trap> Tbl_TrapSetup = new List<Trap>();

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
        for (int i = 0; i < data.Count; i++)
        {
            Trap tmp = new Trap();
            tmp.Level = int.Parse(data[i]["Level"].ToString());
            tmp.Attack = int.Parse(data[i]["Attack"].ToString());
            tmp.Price = int.Parse(data[i]["Price"].ToString());
            tmp.UpgradeCost = int.Parse(data[i]["UpgradeCost"].ToString());
            tmp.CoolTime= double.Parse(data[i]["CoolTime"].ToString());
            tmp.DisassemblyTime = int.Parse(data[i]["DisassemblyTime"].ToString());
            tmp.id = data[i]["id"].ToString();
            tmp.Type = int.Parse(data[i]["Type"].ToString());
            Tbl_TrapSetup.Add(tmp);
        }
    }    
}
