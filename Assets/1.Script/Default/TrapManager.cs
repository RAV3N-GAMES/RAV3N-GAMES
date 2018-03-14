using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;

/*
     * Tbl_EnemySetup Index
     * 0 ~ 21 : HumanTrap
     * 22 ~ 43: Warp
     * 44 ~ 65 : FlameThrowingTrap
     * 66 ~ 87 : ObstructMovementCurrent
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
        /*  입력값 확인용 코드입니다. 지우지 마세요. 
           for (int i = 0; i < 88; i++)
           {
               Debug.Log(i + "th index: " + Tbl_TrapSetup[i].Level + " " + Tbl_TrapSetup[i].Price + " " + Tbl_TrapSetup[i].UpgradeCost+ " " + Tbl_TrapSetup[i].CoolTime+ " " + Tbl_TrapSetup[i].Attack+ " " + Tbl_TrapSetup[i].DisassemblyTime+ " " + Tbl_TrapSetup[i].Type+ " " + Tbl_TrapSetup[i].id + " " + Tbl_TrapSetup[i].SellPrice+ " " + Tbl_TrapSetup[i].ActiveCost + " " + Tbl_TrapSetup[i].EffectContinuousTime);
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
            Trap tmp = new Trap();
            tmp.Level = int.Parse(data[i]["Level"].ToString());
            tmp.Attack = int.Parse(data[i]["Attack"].ToString());
            tmp.Price = int.Parse(data[i]["Price"].ToString());
            tmp.UpgradeCost = int.Parse(data[i]["UpgradeCost"].ToString());
            tmp.CoolTime= double.Parse(data[i]["CoolTime"].ToString());
            tmp.DisassemblyTime = double.Parse(data[i]["DisassemblyTime"].ToString());
            tmp.id = data[i]["id"].ToString();
            tmp.Type = int.Parse(data[i]["Type"].ToString());
            tmp.SellPrice= int.Parse(data[i]["SellPrice"].ToString());
            tmp.ActiveCost= int.Parse(data[i]["ActiveCost"].ToString());
            tmp.EffectContinuousTime= double.Parse(data[i]["EffectContinuousTime"].ToString());
            Tbl_TrapSetup.Add(tmp);
        }
    }    
}
