using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInfo : MonoBehaviour
{
    //public Transform FlameThrowingTrap;
    public GameObject ClickCollider;
    public GameObject TileCollider;
    public GameObject ObjectCollider;
    public GameObject HitCollider;

    public GameObject DamageObjPos;

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

        switch (type)
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

        if (type == 2)
            InitFriend();
    }

    public void InitObject(SaveObject objInfo) //설치된거 껐다 켜고 설치할때
    {
        level = objInfo.level;
        presentHP = objInfo.presentHP;
        totalHP = objInfo.totalHP;
        coordinate = objInfo.coordinate;
        DontDestroy = objInfo.DontDestroy;

        if (type == 2)
            InitFriend();

        isRotation = objInfo.isRotation;

        Rotate(InitDir());

        if (DamageObjPos != null)
        {
            if (presentHP < totalHP)
                DamageObjPos.SetActive(true);
        }

        if (HitCollider != null)
            HitCollider.SetActive(DayandNight.isDay);
        SetHP(0);
        StartCoroutine(Display());
    }

    bool lastColCount()
    {
        if (GetComponent<CheckTile>().lastColList.Count == 0)
            return false;
        else
            return true;
    }


    IEnumerator Display()
    {
        yield return new WaitUntil(lastColCount);
        //yield return null;
        if (GetComponent<CheckTile>().lastColList.Count == 0)
            print("lastCol == 0 tileCol" + TileCollider.activeInHierarchy);
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

    public void SetHP(int changeHP)
    {
        if (changeHP < 0)
        {
            if (DamageObjPos != null)
                DamageObjPos.SetActive(true);

            if (changeHP + presentHP <= 0)
            {
                presentHP = 0;
                GetComponent<DisplayObject>().DestroyObj(true); //게임 오브젝트를 Destory 해야함//나중에 문제 생길수도 있음

                Destroy(gameObject);
            }
            else
                presentHP += changeHP;
        }
        else
        {
            if (DamageObjPos != null)
            {
                if (changeHP + presentHP < totalHP)
                    DamageObjPos.SetActive(true);
            }
        }
    }

    public void RepairObject(int repairHP)
    {
        if (repairHP == -1)
        {
            presentHP = totalHP;
        }
        else if (repairHP > 0)
        {
            presentHP += repairHP;

            if (presentHP > totalHP)
                presentHP = totalHP;
        }

        if (presentHP == totalHP && DamageObjPos != null)
            DamageObjPos.SetActive(false);

        Friendly f = GetComponentInChildren<Friendly>();
        Wall w = GetComponentInChildren<Wall>();
        if (f)
        {
            f.Hp = presentHP;
        }
        if (w)
        {
            w.HP = presentHP;
        }
    }

    public void OnDisplay()
    {
        isDisplay = true;

        //ClickCollider.SetActive(true);
        TileCollider.SetActive(false);
        ClickCollider.SetActive(!DayandNight.isDay);

        GetComponent<ObjectColor>().OffColor();
        GetComponent<ObjectMove>().enabled = false;
    }

    public void OffDisplay()
    {
        Debug.Log("OffDisplay");
        isDisplay = false;

        GetComponent<CheckTile>().lastColList.Clear();

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

    public void SynctoWall()
    {
        Wall w = GetComponentInChildren<Wall>();
        w.Level = level;
        w.HP = presentHP;
        w.MaxHP = totalHP;
        w.WallSyncInfo();
    }

    private void InitFriend()
    {
        Friendly fInfo;

        try
        {
            fInfo = GetComponentInChildren<Friendly>(true);
            switch (fInfo.transform.parent.transform.parent.name)
            {
                case "Guard":
                    fInfo.Hp = presentHP;
                    fInfo.Level = level;
                    fInfo.AttackDamage = OurForcesManager.Tbl_OurForceSetup[fInfo.Level - 1].Attack;
                    fInfo.MaxHp = OurForcesManager.Tbl_OurForceSetup[fInfo.Level - 1].HP;
                    fInfo.AttackEventMax = OurForcesManager.Tbl_OurForceSetup[fInfo.Level - 1].SkillCool;
                    fInfo.scollider.radius = 4;
                    fInfo.StopDistance = (float)OurForcesManager.Tbl_OurForceSetup[fInfo.Level - 1].AttackRange * 2;
                    fInfo.setDelayTime = (float)OurForcesManager.Tbl_OurForceSetup[fInfo.Level - 1].AttackPeriod;
                    fInfo.defaultTime = fInfo.setDelayTime;
                    fInfo.attackDelay = new WaitForSeconds(fInfo.setDelayTime);
                    fInfo.AttackEventMax = OurForcesManager.Tbl_OurForceSetup[fInfo.Level - 1].SkillCool;
                    break;
                case "QuickReactionForces":
                    fInfo.Hp = presentHP;
                    fInfo.Level = level;
                    fInfo.AttackDamage = OurForcesManager.Tbl_OurForceSetup[fInfo.Level + 99].Attack;
                    fInfo.MaxHp = OurForcesManager.Tbl_OurForceSetup[fInfo.Level + 99].HP;
                    fInfo.AttackEventMax = OurForcesManager.Tbl_OurForceSetup[fInfo.Level + 99].SkillCool;
                    fInfo.scollider.radius = 4;
                    fInfo.StopDistance = (float)OurForcesManager.Tbl_OurForceSetup[fInfo.Level + 99].AttackRange * 2;
                    fInfo.setDelayTime = (float)OurForcesManager.Tbl_OurForceSetup[fInfo.Level + 99].AttackPeriod;
                    fInfo.defaultTime = fInfo.setDelayTime;
                    fInfo.attackDelay = new WaitForSeconds(fInfo.setDelayTime);
                    fInfo.AttackEventMax = OurForcesManager.Tbl_OurForceSetup[fInfo.Level + 99].SkillCool;
                    break;
                case "BiochemistryUnit":
                    fInfo.Hp = presentHP;
                    fInfo.Level = level;
                    fInfo.AttackDamage = OurForcesManager.Tbl_OurForceSetup[fInfo.Level + 199].Attack;
                    fInfo.MaxHp = OurForcesManager.Tbl_OurForceSetup[fInfo.Level + 199].HP;
                    fInfo.AttackEventMax = OurForcesManager.Tbl_OurForceSetup[fInfo.Level + 199].SkillCool;
                    fInfo.scollider.radius = 4;
                    fInfo.StopDistance = (float)OurForcesManager.Tbl_OurForceSetup[fInfo.Level + 199].AttackRange * 2;
                    fInfo.setDelayTime = (float)OurForcesManager.Tbl_OurForceSetup[fInfo.Level + 199].AttackPeriod;
                    fInfo.defaultTime = fInfo.setDelayTime;
                    fInfo.attackDelay = new WaitForSeconds(fInfo.setDelayTime);
                    fInfo.AttackEventMax = OurForcesManager.Tbl_OurForceSetup[fInfo.Level + 199].SkillCool;
                    break;
                case "Researcher":
                    fInfo.Hp = presentHP;
                    fInfo.Level = level;
                    fInfo.AttackDamage = OurForcesManager.Tbl_OurForceSetup[fInfo.Level + 299].Attack;
                    fInfo.MaxHp = OurForcesManager.Tbl_OurForceSetup[fInfo.Level + 299].HP;
                    fInfo.AttackEventMax = OurForcesManager.Tbl_OurForceSetup[fInfo.Level + 299].SkillCool;
                    fInfo.scollider.radius = 4;
                    fInfo.StopDistance = (float)OurForcesManager.Tbl_OurForceSetup[fInfo.Level + 299].AttackRange * 2;
                    fInfo.setDelayTime = (float)OurForcesManager.Tbl_OurForceSetup[fInfo.Level + 299].AttackPeriod;
                    fInfo.defaultTime = fInfo.setDelayTime;
                    fInfo.attackDelay = new WaitForSeconds(fInfo.setDelayTime);
                    fInfo.AttackEventMax = OurForcesManager.Tbl_OurForceSetup[fInfo.Level + 299].SkillCool;
                    break;
                default:
                    break;
            }
        }
        catch { }
    }

    private void InitTrap()
    {
    }

    private void InitWall()
    {

    }

    private void InitSecret()
    {

    }
}