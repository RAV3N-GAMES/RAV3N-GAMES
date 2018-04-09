using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInfo : MonoBehaviour
{
    //public Transform FlameThrowingTrap;

    public GameObject ClickCollider;
    public GameObject TileCollider;
    public GameObject ObjectCollider;

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
    public int isRotation;

    public int layerDepth;

    public int DontDestroy;


    public void SetClickColliderPos(float z)
    {
        if (isRotation % 2 == 1)
            ClickCollider.transform.localPosition = new Vector3(0, 0, z);
        else
            ClickCollider.transform.localPosition = new Vector3(0, 0, -z);
    }

    public void InitObject() //새로 생성할때
    {
        string tmpId = id;
        if (id == "Warp_Exit")
            tmpId = "Warp";

        level = JsonDataManager.slotInfoList[tmpId].level;
        
        switch(type)
        {
            case 0:
                totalHP = JsonDataManager.GetBuildingInfo(tmpId, level).HP;
                break;
            case 1: //필요 없을 수 있음 //적군
                totalHP = JsonDataManager.GetEnemyInfo(tmpId, level).HP;
                break;
            case 2:
                totalHP = JsonDataManager.GetOurForcesInfo(tmpId, level).HP;
                break;
            default:
                totalHP = -1;
                break;
        }

        presentHP = totalHP;
    }
    
    public void InitObject(SaveObject objInfo) //설치된거 껐다 켜고 설치할때
    {
        level = objInfo.level;
        presentHP = objInfo.presentHP;
        totalHP = objInfo.totalHP;
        coordinate = objInfo.coordinate;
        DontDestroy = objInfo.DontDestroy;

        isRotation = objInfo.isRotation;

        Rotate(InitDir());

        StartCoroutine(Display());
    }
    
    IEnumerator Display()
    {
        yield return null;
        OnDisplay();
        yield break;
    }

    int SetDir()
    {
        int dir = 1;

        if (isRotation % 2 == 1)
        {
            dir = -1;
        }

        return dir;
    }

    int InitDir()
    {
        return isRotation % 2;
    }

    void Rotate(int dir)
    {
        //if (id.Equals("FlameThrowingTrap"))
        //{
        //    if (isRotation < 2)
        //        FlameThrowingTrap.localPosition = new Vector3(-0.2f, 0.9f, 0);
        //    else
        //        FlameThrowingTrap.localPosition = new Vector3(-0.2f, 0.6f, 0);
        //}

        transform.Rotate(new Vector3(dir * 180, 0, dir * 180));
    }

    public void rotationObject()
    {
        //if (id.Equals("FlameThrowingTrap"))
        //    isRotation = (isRotation + 1) % 4;
        //else    
        isRotation = (isRotation + 1) % 2;
        Rotate(SetDir());

        for (int i = 0; i < coordinate.Length; i = i + 2)
        {
            int temp = coordinate[i];
            coordinate[i] = coordinate[i + 1];
            coordinate[i + 1] = temp;
        }
    }

    public void RepairObject(int repairHP)
    {
        if (repairHP == -1)
            presentHP = totalHP;
        else
        {
            presentHP += repairHP;

            if (presentHP > totalHP)
                presentHP = totalHP;
        }
    }

    public void OnDisplay()
    {
        isDisplay = true;

        ClickCollider.SetActive(true);
        TileCollider.SetActive(false);
        ClickCollider.SetActive(!DayandNight.isDay);
        
        GetComponent<ObjectColor>().OffColor();
    }

    public void OffDisplay()
    {
        isDisplay = false;

        GetComponent<CheckTile>().lastCol.Clear();

        ClickCollider.SetActive(false);
        TileCollider.SetActive(true);
        //ObjectCollider.SetActive(false);
    }

    public void OnDependency()
    {
        DontDestroy++;
    }

    public void OffDepency()
    {
        DontDestroy--;
    }
}
