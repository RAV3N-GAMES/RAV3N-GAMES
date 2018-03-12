using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInfo : MonoBehaviour
{
    public string id;
    public int type;

    public int level;
    public int presentHP;
    public int totalHP;
    //상대적인 좌표
    //(0,1), (2,3) .... 이런식으로 서로 쌍인 좌표
    public int[] coordinate;
    public Transform pivotObject;

    public bool isDisplay;
    public bool isRotation;

    public int layerDepth;

    public void InitObject() //새로 생성할때
    {
        level = JsonDataManager.slotInfoList[id].level;

        switch (type)
        {
            case 0:
                totalHP = JsonDataManager.GetBuildingInfo(id, level).HP;
                break;
            case 1: //필요 없을 수 있음 //적군
                totalHP = JsonDataManager.GetEnemyInfo(id, level).HP;
                break;
            case 2:
                totalHP = JsonDataManager.GetOurForcesInfo(id, level).HP;
                break;
            default:
                totalHP = -1;
                break;
        }

        presentHP = totalHP;
    }

    public void InitObject(SaveObject objInfo) //설치된거 껐다 켜고 설치할때
    {
        id = objInfo.id; //아마 ㄴ필요 
        type = objInfo.type; //아마 ㄴ필요 
        level = objInfo.level;
        presentHP = objInfo.presentHP;
        totalHP = objInfo.totalHP;
        coordinate = objInfo.coordinate;

        if (objInfo.isRotation)
        {
            isRotation = !objInfo.isRotation;
            Rotate();
        }

        GetComponent<ObjectColor>().OffColor();
    }

    void Rotate()
    {
        isRotation = !isRotation;
        int dir = 1;
        if (isRotation)
        {
            dir = -1;
        }

        transform.Rotate(new Vector3(dir * 180, 0, dir * 180));
    }

    public void rotationObject()
    {
        Rotate();

        for (int i = 0; i < coordinate.Length; i = i + 2)
        {
            int temp = coordinate[i];
            coordinate[i] = coordinate[i + 1];
            coordinate[i + 1] = temp;
        }


    }

    public void RepairObject()
    {
        print("repair");

        presentHP = totalHP;
        //돈쓰기
    }

    public void OnDisplay()
    {
        isDisplay = true;
        //여기에서 ObjectInfo 로드

        GetComponent<CheckTile>().OnCheckTile();
        GetComponent<ObjectColor>().OffColor();
    }

    //public void LevelUp()
    //{
    //    //id에 맞는 data 읽어오면 됨
    //    level++;
    //    //MyObject myObject = JsonDataManager.GetObjectInfo(id, level);
    //
    //    //presentHP = myObject.HP;
    //    //totalHP = myObject.HP;
    //}
}