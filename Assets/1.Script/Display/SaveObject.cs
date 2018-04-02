using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveObject {
    public int type;
    public string id;

    public int DontDestroy;

    public int level;
    public int presentHP;
    public int totalHP;

    public int mRow;
    public int mCol;

    public int[] coordinate;

    public string pivotObject;

    public string pos;

    public int isRotation;

    //Warp의 경우에만 사용
    public int parentRow;
    public int parentCol;

    public SaveObject(Vector3 pos, int DontDestroy, int type, string id, int level, int presentHP, int totalHP, 
                       int row, int col, int[] coordinate, string pivotObject, int isRotation, int parentRow, int parentCol)
    {
        this.DontDestroy = DontDestroy;
        this.pos = pos.x + "/" + pos.y + "/" + pos.z;
        this.type = type;
        this.id = id;
        this.level = level;
        this.presentHP = presentHP;
        this.totalHP = totalHP;

        mRow = row;
        mCol = col;

        this.coordinate = coordinate;
        this.pivotObject = pivotObject;
        this.isRotation = isRotation;

        this.parentRow = parentRow;
        this.parentCol = parentCol;
    }
}
