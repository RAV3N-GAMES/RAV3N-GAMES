using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInfo : MonoBehaviour
{
    public string id;

    public int level;
    public int presentHP;
    public int totalHP;

    //상대적인 좌표
    //(0,1), (2,3) .... 이런식으로 서로 쌍인 좌표

    public bool isDisplay;
    public int[] coordinate;

    public Transform pivotObject;

    void Awake()
    {
        SetPivotObject();
    }

    public void InitObject(int HP, int level)
    {
        this.level = level;

        presentHP = HP;
        totalHP = HP;
    }

    void SetPivotObject()
    {
        pivotObject = transform.GetChild(0);
        for (int i = 1; i < transform.childCount; i++)
        {
            if (pivotObject.transform.position.y > transform.GetChild(i).position.y)
            {
                pivotObject = transform.GetChild(i);
				Debug.Log(pivotObject.position);
			}
		}
    }

    public void rotationObject() //회전
    {
        for (int i = 0; i < coordinate.Length; i = i + 2)
        {
            int temp = coordinate[i];
            coordinate[i] = coordinate[i + 1];
            coordinate[i + 1] = temp;
        }

        SetPivotObject();
    }

    public void RepairObject()
    {
        print("repair");
    }

    public void OnDisplay()
    {
        isDisplay = true;
        GetComponent<ObjectColor>().OffColor();
    }
}
