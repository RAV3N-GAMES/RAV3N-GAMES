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

    public void UsingTile(int[] idx)
    {
        tileManager.UsingTile(gameObject, idx);
    }

    public void DestryObj(int[] idx)
    {
        tileManager.DestroyObj(gameObject, idx);
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

    int[] makeObjIdx()
    {
        int[] idx = new int[lastCol.Count * 2];

        for (int i = 0; i < idx.Length; i = i + 2)
        {
            idx[i] = System.Int32.Parse(lastCol[i / 2].transform.parent.name);
            idx[i + 1] = System.Int32.Parse(lastCol[i / 2].name);
        }

        return idx;
    }

    bool isEnable()
    {
        if (lastCol.Count == 0 || !isSameRoom() || lastCol.Count < TileCnt)
            return false;

        int[] idx = makeObjIdx();

        tileManager = lastCol[0].transform.parent.parent.GetComponent<TileManager>();
        return tileManager.isEnableTile(idx);
    }

    public void OnCheckTile()
    {
        isPossible = isEnable();        
        objectColor.OnColor(isPossible);
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Tile")
        {
            lastCol.Add(col.gameObject);

            if (!objectInfo.isDisplay)
                OnCheckTile();
        }
    }

    void OnCollisionExit(Collision col)
    {
        if (col.gameObject.tag == "Tile")
        {
            lastCol.Remove(col.gameObject);

            if (!objectInfo.isDisplay)
                OnCheckTile();
        }
    }
}
