using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileObject {
    public GameObject mObject { get; } //이거 변경해줘야하나

    ObjectInfo objectInfo;

    public int mRow { get; } //바닥타일의 0을 기준으로
    public int mCol { get; } //나중에는 변경가능해야함

    public TileObject(GameObject _Obj, int row, int col)
    {
        mObject = _Obj;
        mRow = row;
        mCol = col;

        objectInfo = mObject.GetComponent<ObjectInfo>();
    }

    public void OnTransparency(bool isTransparency)
    {
        if (objectInfo.type == 0)
            mObject.GetComponent<ObjectColor>().OnTransparency(isTransparency);
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
