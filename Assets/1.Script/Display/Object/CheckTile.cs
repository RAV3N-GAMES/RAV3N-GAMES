using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckTile : MonoBehaviour
{
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

    public void UsingTile(int[] idx)
    {
        if (isPossible)
            tileManager.UsingTile(gameObject, idx);

    }

    public void DestryObj(int[] idx)
    {
        tileManager.DestroyObj(GetComponent<ObjectInfo>().layerDepth, idx);
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

    int[] makeIdx()
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

    bool isBuilding_OMC()
    {
        Transform pivot = findPivotCol();

        int row = System.Int32.Parse(pivot.transform.parent.name);
        int col = System.Int32.Parse(pivot.name);

        int[] displayTile = new int[4];

        if (!objectInfo.isRotation)
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

        return tileManager.isBuilding(displayTile);

    }
    //bool isBuilding_SRF()
    //{
    //    Transform pivot = findPivotCol();
    //
    //    int row = System.Int32.Parse(pivot.transform.parent.name);
    //    int col = System.Int32.Parse(pivot.name);
    //
    //    int[] displayTile = new int[4];
    //
    //    if (!objectInfo.isRotation)
    //    {
    //        displayTile[0] = row;
    //        displayTile[1] = col + 2;
    //        displayTile[2] = row + 1;
    //        displayTile[3] = col + 2;
    //    }
    //    else
    //    {
    //        displayTile[0] = row + 2;
    //        displayTile[1] = col;
    //        displayTile[2] = row + 2;
    //        displayTile[3] = col + 1;
    //    }
    //
    //    return tileManager.isBuilding(displayTile);
    //}

    bool isBuilding_FTT()
    {
        Transform pivot = findPivotCol();

        int row = System.Int32.Parse(pivot.transform.parent.name);
        int col = System.Int32.Parse(pivot.name);

        int[] displayTile = new int[2];

        if (!objectInfo.isRotation)
        {
            displayTile[0] = row;
            displayTile[1] = col + 1;
        }
        else
        {
            displayTile[0] = row + 1;
            displayTile[1] = col;
        }

        return tileManager.isBuilding(displayTile);
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
        //else if(name == "SpaceVoiceRecordingFile")
        //{
        //    isBuilding = isBuilding_SRF();
        //}
        else if (name == "FlameThrowingTrap")
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
            lastCol.Add(col.transform.parent.gameObject);
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "Tile")
        {
            lastCol.Remove(col.transform.parent.gameObject);
        }
    }
}