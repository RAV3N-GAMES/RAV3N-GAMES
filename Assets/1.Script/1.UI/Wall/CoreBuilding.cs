using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreBuilding : Wall {

    private Friendly F;

    private void FriendlyAttackRangeUp()
    {
        //Range Up
        /*
         방 전체 공격
         방 끝에 걸쳐있으면???
          -> 원래대로면 옆방 때릴 수 있는데
           이러면 못 때리는건지??
          */
        if (F.name == "BiochemistryUnit" || F.name == "QuickReactionForces")
        {
            F.StopDistance = 30;
            F.TargetIdx =int.Parse(GetComponentInParent<CheckTile>().tileManager.name);
        }
    }

    protected override void WallInit()
    {
        base.WallInit();
        compensation = 299;
        MaxHP = BuildingManager.Tbl_BuildingSetup[Level + compensation].HP;
        Price = BuildingManager.Tbl_BuildingSetup[Level + compensation].Price;
        UpgraeCost = BuildingManager.Tbl_BuildingSetup[Level + compensation].UpgradeCost;
        RepairCost = BuildingManager.Tbl_BuildingSetup[Level + compensation].RepairCost;
        ActiveCost = BuildingManager.Tbl_BuildingSetup[Level + compensation].ActiveCost;
    }

    // Use this for initialization
    void Start()
    {
        WallInit();
    }

    // Update is called once per frame
    void Update () {
		
	}
}
