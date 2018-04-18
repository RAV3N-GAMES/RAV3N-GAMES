using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldBuilding : Wall {
    protected override void WallInit()
    {
        base.WallInit();
        MaxHP=BuildingManager.Tbl_BuildingSetup[Level-1].HP;
        Price = BuildingManager.Tbl_BuildingSetup[Level - 1].Price;
        UpgraeCost= BuildingManager.Tbl_BuildingSetup[Level - 1].UpgradeCost;
        RepairCost= BuildingManager.Tbl_BuildingSetup[Level - 1].RepairCost;
        ActiveCost= BuildingManager.Tbl_BuildingSetup[Level - 1].ActiveCost;
    }
    
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
