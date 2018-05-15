using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileObject {
    public GameObject mObject { get; } //이거 변경해줘야하나
    ObjectInfo objectInfo;

    public int mRow { get; } //바닥타일의 0을 기준으로
    public int mCol { get; } //나중에는 변경가능해야함

    public int parentRow;
    public int parentCol;


    public TileObject(GameObject _Obj, int row, int col, int parentRow, int parentCol)
    {
        mObject = _Obj;
        mRow = row;
        mCol = col;

        this.parentRow = parentRow;
        this.parentCol = parentCol;

        objectInfo = mObject.GetComponent<ObjectInfo>();
    }

    public SaveObject GetSaveObj()
    {
        return new SaveObject(mObject.transform.position, objectInfo.DontDestroy, objectInfo.type, objectInfo.id, objectInfo.level,
                             objectInfo.presentHP, objectInfo.totalHP, mRow, mCol, objectInfo.coordinate, objectInfo.pivotObject.gameObject.name,
                          objectInfo.isRotation, parentRow, parentCol);
    }

    public int GetObjectType()
    {
        if (mObject.GetComponent<ObjectInfo>().id == "Warp_Exit")
            return -1;
        return mObject.GetComponent<ObjectInfo>().type;
    }

    public void Repair()
    {
        objectInfo.RepairObject(-1);
    }

    public int GetRepairCost()
    {
        int damageHP = objectInfo.totalHP - objectInfo.presentHP;
        int repairCost = 0;

        switch (objectInfo.type)
        {
            case 0:
                repairCost =  damageHP * JsonDataManager.GetBuildingInfo(objectInfo.id, objectInfo.level).RepairCost;
                break;
            case 2:
                repairCost = (int)(damageHP * JsonDataManager.GetOurForcesInfo(objectInfo.id, objectInfo.level).HealCost);
                break;
        }

        if(repairCost != 0)
        {
            if (objectInfo.type == 0)
                MapManager.DamageBuildingCnt++;
            else if (objectInfo.type == 2)
                MapManager.DamageOurForcesCnt++;
        }

        return repairCost;
    }

    public void OnOffHitCollider()
    {
        try
        {
            mObject.GetComponent<ObjectInfo>().HitCollider.SetActive(DayandNight.isDay);
        }
        catch{ }
    }

    public void OnTransparency(bool isTransparency)
    {
        if (objectInfo.type == 0)
        {
            mObject.GetComponent<ObjectColor>().OnTransparency(isTransparency);
            SetClickColliderStatus(!isTransparency);
        }
    }

    public void SetClickColliderStatus(bool isActive)
    {
        if (objectInfo.type == 0)
        {
            if (RoomManager.isTransparency && isActive)
                return;
        }
        mObject.GetComponent<ObjectInfo>().ClickCollider.SetActive(isActive);
    }

    public void SetOrderInLayer(string layer, int idx, int layerDepth)
    {
        mObject.GetComponent<ObjectColor>().SetSortingOrder(layer, idx);
        SetLayerDepth(layerDepth);
    }

    public void SetLayerDepth(int layerDepth)
    {
        objectInfo.layerDepth = layerDepth;
    }

    public float[] CenterCoordinate()
    {
        float[] center = new float[2];

        int coorLength = objectInfo.coordinate.Length;

        center[0] = mRow + ((float)objectInfo.coordinate[coorLength - 2] / 2f);
        center[1] = mCol + ((float)objectInfo.coordinate[coorLength - 1] / 2f);

        return center;
    }

    public bool isTileBuilding(int row, int col)
    {
        if (objectInfo.type == 0)
        {
            int[] idx = mObject.GetComponent<CheckTile>().makeIdx();

            for (int i = 0; i < idx.Length; i = i + 2)
            {
                if (idx[i] == row && idx[i + 1] == col)
                        return true;
            }
            return false;
        }
        else
            return false;
    }

    public void OnDependency()
    {
        objectInfo.OnDependency();
    }

    public void OffDepency()
    {
        objectInfo.OffDepency();
    }
}
