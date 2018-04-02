using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckTile : MonoBehaviour {
    [HideInInspector]
    public List<GameObject> lastCol;

    ObjectInfo objectInfo;
    ObjectColor objectColor;
    DisplayObject displayObject;

    [HideInInspector]
    public TileManager tileManager;

    public bool isPossible;

    int TileCnt;

    void Start()
    {
        lastCol = new List<GameObject>();

        objectInfo = GetComponent<ObjectInfo>();
        objectColor = GetComponent<ObjectColor>();
        displayObject = GetComponent<DisplayObject>();

        TileCnt = objectInfo.coordinate.Length / 2;
    }

    public Transform findPivotCol()
    {
        int Cnt = lastCol.Count;

        GameObject tempPivot = lastCol[0];

        for (int i = 1; i < Cnt; i++)
        {
            if (lastCol[i].transform.position.z < tempPivot.transform.position.z)
            {
                tempPivot = lastCol[i];
            }
        }

        return tempPivot.transform;
    }

    public bool UsingTile(int[] idx)
    {
        OnDisplayCheckTile();

        if (isPossible) {
            if (name == "ObstructMovementCurrent")
            {
                int[] omcIdx = DepencyTile_OMC();

                tileManager.OnDependency(omcIdx[0], omcIdx[1]);
                tileManager.OnDependency(omcIdx[2], omcIdx[3]);
            }
            else if (name == "FlameThrowingTrap")
            {
                int[] fttIdx = DepencyTile_FTT();

                tileManager.OnDependency(fttIdx[0], fttIdx[1]);
            }

            tileManager.UsingTile(gameObject, idx);
            return true;
        }
        return false;
    }

    public bool DestroyObj(int[] idx)
    {
        return tileManager.DestroyObj(gameObject, idx);
    }

    bool isSameRoom()
    {
        if (lastCol.Count == 1)
            return true;

        string roomName = lastCol[0].transform.parent.parent.name;

        for (int i = 1; i < lastCol.Count; i++)
        {
            if (roomName != lastCol[i].transform.parent.parent.name)
                return false;
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
    public int[] DepencyTile_OMC()
    {
        Transform pivot = findPivotCol();

        int row = System.Int32.Parse(pivot.transform.parent.name);
        int col = System.Int32.Parse(pivot.name);

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
        return tileManager.isBuilding(DepencyTile_OMC());
    }

    public int[] DepencyTile_FTT()
    {
        Transform pivot = findPivotCol();

        int row = System.Int32.Parse(pivot.transform.parent.name);
        int col = System.Int32.Parse(pivot.name);

        int[] displayTile = new int[2];

        if (objectInfo.isRotation == 0)
        {
            displayTile[0] = row;
            displayTile[1] = col + 1;
        }
        else if(objectInfo.isRotation == 1)
        {
            displayTile[0] = row + 1;
            displayTile[1] = col;
        }
        else if(objectInfo.isRotation == 2)
        {
            displayTile[0] = row - 1;
            displayTile[1] = col;
        }
        else if (objectInfo.isRotation == 3)
        {
            displayTile[0] = row;
            displayTile[1] = col - 1;
        }

        return displayTile;
    }

    bool isBuilding_FTT()
    {
        return tileManager.isBuilding(DepencyTile_FTT());
    }

    bool isEnable()
    {
        if (lastCol.Count == 0 || !isSameRoom() || lastCol.Count < TileCnt)
            return false;

        tileManager = lastCol[0].transform.parent.parent.GetComponent<TileManager>();

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
                lastCol.Add(col.transform.parent.gameObject);
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
                lastCol.Remove(col.transform.parent.gameObject);
            }

        }
    }
}
