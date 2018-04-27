using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckTile : MonoBehaviour {
    [HideInInspector]
    public List<GameObject> lastColList;

    GameObject lastCol;

    public SpriteRenderer Warp;
    public SpriteRenderer WarpExit;

    ObjectInfo objectInfo;
    ObjectColor objectColor;
    DisplayObject displayObject;

    [HideInInspector]
    public TileManager tileManager;

    public bool isPossible;

    int TileCnt;

    void Start()
    {
        lastColList = new List<GameObject>();

        objectInfo = GetComponent<ObjectInfo>();
        objectColor = GetComponent<ObjectColor>();
        displayObject = GetComponent<DisplayObject>();

        TileCnt = objectInfo.coordinate.Length / 2;
    }

    public Transform findPivotCol()
    {
        int Cnt = lastColList.Count;
        if (Cnt == 0)
            return lastCol.transform;

        GameObject tempPivot = lastColList[0];

        for (int i = 1; i < Cnt; i++)
        {
            if (lastColList[i].transform.position.z < tempPivot.transform.position.z)
            {
                tempPivot = lastColList[i];
            }
        }

        return tempPivot.transform;
    }

    int[] AroundBuildingIdx()
    {
        int[] buildingIdx = makeIdx();
        int[] dir = { 0, 1, 1, 0, 0, -1, -1, 0 };

        int[] aroundBuildingIdx;
        if (buildingIdx.Length == 8)
            aroundBuildingIdx = new int[16];
        else //NewBuilding
            aroundBuildingIdx = new int[20];

        int Cnt = 0;
        for (int i = 0; i < buildingIdx.Length; i = i + 2)
        {
            for(int j = 0; j < dir.Length; j = j + 2)
            {
                int row = buildingIdx[i] + dir[j];
                int col = buildingIdx[i + 1] + dir[j + 1];

                int k = 0;
                for (; k < buildingIdx.Length; k = k + 2)
                {
                    if (buildingIdx[k] == row)
                        if (buildingIdx[k + 1] == col)
                            break;
                }

                if (k == buildingIdx.Length)
                {
                    aroundBuildingIdx[Cnt] = row;
                    aroundBuildingIdx[Cnt + 1] = col;

                    Cnt = Cnt + 2;
                }
            }
        }

        return aroundBuildingIdx;
    }

    int[] GetPivotCoordinate()
    {
        Transform pivot = findPivotCol();
        int[] pivotCoordinate = { System.Int32.Parse(pivot.transform.parent.name), System.Int32.Parse(pivot.name) };

        return pivotCoordinate;
    }

    public bool UsingTile(int[] idx)
    {
        OnDisplayCheckTile();

        if (isPossible) {
            if (objectInfo.type == 0)
            {
                objectInfo.DontDestroy += tileManager.isFlameThrowingTrap(AroundBuildingIdx());
            }
            else if (name.Equals("ObstructMovementCurrent"))
            {
                int[] omcIdx = DepencyTile_OMC(GetPivotCoordinate());

                tileManager.OnDependency(omcIdx[0], omcIdx[1]);
                tileManager.OnDependency(omcIdx[2], omcIdx[3]);
            }
            else if (name.Equals("FlameThrowingTrap"))
            {
                int[] fttIdx = DepencyTile_FTT(GetPivotCoordinate());

                for(int i = 0; i < fttIdx.Length; i = i + 2)
                {
                    tileManager.OnDependency(fttIdx[i], fttIdx[i + 1]);
                }
            }

            tileManager.UsingTile(gameObject, idx);
            return true;
        }
        return false;
    }

    public bool DestroyObj(bool isDestroyed , int[] idx)
    {
        return tileManager.DestroyObj(isDestroyed, gameObject, idx);
    }

    bool isSameRoom()
    {
        if (lastColList.Count == 1)
            return true;

        string roomName = lastColList[0].transform.parent.parent.name;

        for (int i = 1; i < lastColList.Count; i++)
        {
            if (roomName != lastColList[i].transform.parent.parent.name)
                return false;
        }

        if (name.Equals("Warp_Exit"))
        {
            if (!roomName.Equals(Warp.sortingLayerName))
                return false;
        }
        else if(name.Equals("Warp"))
        {
            if(!WarpExit.sortingLayerName.Equals("NewObject"))
            {
                if (!roomName.Equals(WarpExit.sortingLayerName))
                    return false;
            }
        }

        return true;
    }

    public int[] makeIdx()
    {
        Transform pivot = findPivotCol();

        int[] coordinate = GetComponent<ObjectInfo>().coordinate;
        int[] idx = new int[coordinate.Length];

        idx[0] = System.Int32.Parse(pivot.transform.parent.name);
        idx[1] = System.Int32.Parse(pivot.name);

        for (int i = 0; i < idx.Length; i = i + 2)
        {
            idx[i] = idx[0] + coordinate[i];
            idx[i + 1] = idx[1] + coordinate[i + 1];
        }

        return idx;
    }
    public int[] DepencyTile_OMC(int[] pivotCoordinte)
    {
        int row = pivotCoordinte[0];
        int col = pivotCoordinte[1];

        int[] displayTile = new int[4];

        if (objectInfo.isRotation == 0)
        {
            displayTile[0] = row;
            displayTile[1] = col - 1;
            displayTile[2] = row;
            displayTile[3] = col + 5;
        }
        else
        {
            displayTile[0] = row - 1;
            displayTile[1] = col;
            displayTile[2] = row + 5;
            displayTile[3] = col;
        }

        return displayTile;
    }

    bool isBuilding_OMC()
    {
        return tileManager.isBuilding(DepencyTile_OMC(GetPivotCoordinate()));
    }

    public int[] DepencyTile_FTT(int[] pivotCoordinate)
    {
        int row = pivotCoordinate[0];
        int col = pivotCoordinate[1];

        int[] displayTile = { row, col + 1, row + 1, col, row - 1, col, row, col - 1 };
        List<int> tileIdx = new List<int>();

        for(int i = 0; i < displayTile.Length; i = i + 2)
        {
            int[] tempTile = { displayTile[i], displayTile[i + 1] };

            if (tileManager.isBuilding(tempTile))
            {
                tileIdx.Add(i);
                tileIdx.Add(i + 1);
            }
        }

        if (tileIdx.Count == 0)
            return null;

        int[] dependencyTile = new int[tileIdx.Count];

        for(int i = 0; i < dependencyTile.Length; i++)
        {
            dependencyTile[i] = displayTile[tileIdx[i]];
        }

        return dependencyTile;
    }

    bool isBuilding_FTT()
    {
        if (DepencyTile_FTT(GetPivotCoordinate()) != null)
            return true;
        else
            return false;
    }

    bool isEnable()
    {
        if (lastColList.Count == 0 || !isSameRoom() || lastColList.Count < TileCnt)
            return false;        

        tileManager = lastColList[0].transform.parent.parent.GetComponent<TileManager>();

        int[] idx = makeIdx();
        return tileManager.isEnableTile(idx);
    }

    public void OnDisplayCheckTile() //Rotation할때랑 내려놓았을때. 설치되는 경우에만 사용 (움직이는동안 사용 x)
    {
        bool isBuilding = true;
        if (name == "ObstructMovementCurrent")
        {
            isBuilding = isBuilding_OMC();
        }
        else if(name == "FlameThrowingTrap")
        {
            isBuilding = isBuilding_FTT();
        }

        isPossible = isEnable() & isBuilding;
        
        objectColor.OnColor(isPossible);
    }

    public void OnCheckTile()
    {
        isPossible = isEnable();
        objectColor.OnColor(isPossible);
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Tile")
        {
            if (!objectInfo.isDisplay)
            {
                lastColList.Add(col.transform.parent.gameObject);
                OnCheckTile();
            }
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "Tile")
        {
            if (!objectInfo.isDisplay)
            {
                if (lastColList.Count == 1)
                    lastCol = lastColList[0];
                lastColList.Remove(col.transform.parent.gameObject);
            }
        }
    }
}
