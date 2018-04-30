using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBuilding : Wall {
    protected override void WallInit()
    {
        base.WallInit();
        compensation = 99;
        MaxHP = BuildingManager.Tbl_BuildingSetup[Level + compensation].HP;
        Price = BuildingManager.Tbl_BuildingSetup[Level + compensation].Price;
        UpgraeCost = BuildingManager.Tbl_BuildingSetup[Level + compensation].UpgradeCost;
        RepairCost = BuildingManager.Tbl_BuildingSetup[Level + compensation].RepairCost;
        ActiveCost = BuildingManager.Tbl_BuildingSetup[Level + compensation].ActiveCost;
    }

    // Use this for initialization
    void Start () {
        WallInit();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
