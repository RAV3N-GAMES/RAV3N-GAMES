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
    public int[] coordinate;
    public Transform pivotObject;

    public bool isDisplay;

    void Awake()
    {
        SetPivotObject();
    }

    public void InitObject(SaveObject objInfo)
    {
        id = objInfo.id;
        level = objInfo.level;
        presentHP = objInfo.presentHP;
        totalHP = objInfo.totalHP;


        //회전시키고 나서 위치 안맞을수도 있겠다. 그건 맞춰주면 되고
        while (objInfo.pivotObject != pivotObject.name)
        {
            for (int i = 0; i < coordinate.Length; i++)
            {
                if (coordinate[i] != objInfo.coordinate[i])
                {
                    rotationObject();
                    break;
                }
            }
        }
    }
    
    void SetPivotObject()
    {
        pivotObject = transform.GetChild(0);
        for (int i = 1; i < transform.childCount; i++)
        {
            if (pivotObject.transform.position.y > transform.GetChild(i).position.y)
            {
                pivotObject = transform.GetChild(i);
            }
        }
    }

    public void rotationObject() //회전 //함수 자체 수정해야 할 수 있음 //그냥 다
    {
        for (int i = 0; i < coordinate.Length; i = i + 2)
        {
            int temp = coordinate[i];
            coordinate[i] = coordinate[i + 1];
            coordinate[i + 1] = temp;
        }
        //실제로 오브젝트 위치 회전 시키는거 해야함
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

    public void LevelUp()
    {
        //id에 맞는 data 읽어오면 됨
    }
}
