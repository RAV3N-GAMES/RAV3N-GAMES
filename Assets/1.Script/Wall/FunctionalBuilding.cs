using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FunctionalBuilding : Wall {
    private GameObject[] AllTrap;
    private List<Trap> ProtectedTraps;
    private const int ProtectRange = 3;

    private void SelectTraps(GameObject[] AllTrap)
    {
        for (int i = 0; i < AllTrap.Length; i++)
        {
            if (Vector3.Distance(transform.position, ProtectedTraps[i].transform.position) > ProtectRange)
            {
                ProtectedTraps.Add(AllTrap[i].GetComponentInChildren<Trap>());
                AllTrap[i].GetComponent<ObjectInfo>().DontDestroy = 1;
            }
        }
    }

    private void FreeTraps()
    {
        for (int i = 0; i < ProtectedTraps.Count; i++)
        {
            ProtectedTraps[i].GetComponentInParent<ObjectInfo>().DontDestroy = 0;
        }
    }

    public override void DestoryWall()
    {
        FreeTraps();
        base.DestoryWall();
    }
    protected override void WallInit()
    {
        base.WallInit();
        compensation = 199;
        MaxHP = BuildingManager.Tbl_BuildingSetup[Level + compensation].HP;
        Price = BuildingManager.Tbl_BuildingSetup[Level + compensation].Price;
        UpgraeCost = BuildingManager.Tbl_BuildingSetup[Level + compensation].UpgradeCost;
        RepairCost = BuildingManager.Tbl_BuildingSetup[Level + compensation].RepairCost;
        ActiveCost = BuildingManager.Tbl_BuildingSetup[Level + compensation].ActiveCost;
        AllTrap = GameObject.FindGameObjectsWithTag("Trap");
        SelectTraps(AllTrap);
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
