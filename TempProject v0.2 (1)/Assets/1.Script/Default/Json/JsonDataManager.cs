using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using LitJson;


public class JsonDataManager : MonoBehaviour {
    public static Dictionary<string, SlotInfo> slotInfoList { get; private set; }   //현재까지 오브젝트 업데이트 정보및 슬롯관련 정보
                                                                                    //<id, SlotInfo>의 데이터 쌍
    public static Dictionary<string, Sprite> slotImage { get; private set; }

    void Awake()
    {
        slotInfoList = new Dictionary<string, SlotInfo>();
        slotImage = new Dictionary<string, Sprite>();

        Data_Player.addGold(10000);

        LoadData();
    }

    SlotInfo InitSlotInfo(JsonData data)
    {
        int type = int.Parse(data["type"].ToString());
        string id = data["id"].ToString();
        int level = int.Parse(data["level"].ToString());
        int price = int.Parse(data["price"].ToString());
        string imageName = data["imageName"].ToString();

        return new SlotInfo(type, id, price, level, imageName);
    }

    void LoadSlotData()
    {
        string slotObj = File.ReadAllText(Application.dataPath + "/Resources/Data/SlotInfo.json");
        JsonData slotData = JsonMapper.ToObject(slotObj);

        for (int i = 0; i < slotData.Count; i++)
        {
            SlotInfo newSlot = InitSlotInfo(slotData[i]);
            slotInfoList.Add(newSlot.id, newSlot);

            Sprite newSprite;
            try
            {
                newSprite = Resources.Load<GameObject>("Image/" + newSlot.imageName).GetComponent<SpriteRenderer>().sprite;
            }
            catch
            {
                newSprite = Resources.Load<GameObject>("Image/OldBuilding").GetComponent<SpriteRenderer>().sprite;
            }

            slotImage.Add(newSlot.imageName, newSprite);
        }
    }
    

    void LoadData()
    {
        LoadSlotData();
    }

    static void SaveData()
    {
        List<SlotInfo> tmp = new List<SlotInfo>();
        
        foreach(var slot in slotInfoList)
        {
            tmp.Add(slot.Value);
        }

        JsonData newObj = JsonMapper.ToJson(tmp);
        File.WriteAllText(Application.dataPath + "/Resources/Data/SlotInfo.json", newObj.ToString());
    }
    
    public static Building GetBuildingInfo(string id, int level)
    {
        for (int i = 0; i < BuildingManager.Tbl_BuildingSetup.Count; i++)
        {
            if(BuildingManager.Tbl_BuildingSetup[i].id == id)
            {
                if(BuildingManager.Tbl_BuildingSetup[i].Level == level)
                    return BuildingManager.Tbl_BuildingSetup[i];
            }
        }
    
        return null;
    }

    public static Enemy GetEnemyInfo(string id, int level)
    {
        for (int i = 0; i < EnemyManager.Tbl_EnemySetup.Count; i++)
        {
            if (EnemyManager.Tbl_EnemySetup[i].id == id)
            {
                if (EnemyManager.Tbl_EnemySetup[i].Level == level)
                    return EnemyManager.Tbl_EnemySetup[i];
            }
        }

        return null;
    }

    public static OurForces GetOurForcesInfo(string id, int level)
    {
        for (int i = 0; i < OurForcesManager.Tbl_OurForceSetup.Count; i++)
        {
            if (OurForcesManager.Tbl_OurForceSetup[i].id == id)
            {
                if (OurForcesManager.Tbl_OurForceSetup[i].Level == level)
                    return OurForcesManager.Tbl_OurForceSetup[i];
            }
        }

        return null;
    }

    public static Secret GetSecretInfo(string id, int fame)
    {
        for (int i = 0; i < SecretManager.Tbl_SecretSetup.Count; i++)
        {
            if (SecretManager.Tbl_SecretSetup[i].id == id)
            {
                if (SecretManager.Tbl_SecretSetup[i].Fame == fame)
                    return SecretManager.Tbl_SecretSetup[i];
            }
        }

        return null;
    }

    public static Trap GetTrapInfo(string id, int level)
    {
        for (int i = 0; i < TrapManager.Tbl_TrapSetup.Count; i++)
        {
            if (TrapManager.Tbl_TrapSetup[i].id == id)
            {
                if (TrapManager.Tbl_TrapSetup[i].Level == level)
                    return TrapManager.Tbl_TrapSetup[i];
            }
        }

        return null;
    }

    public static void SetSlotInfo(int Type, string id, int level)
    {
        slotInfoList[id].level = level;

        //0 : 건물
        //1 : 적군
        //2 : 아군
        //3 : 함정
        //4 : 기밀
        try
        {
            switch (Type)
            {
                case 0:
                    slotInfoList[id].price = GetBuildingInfo(id, level).Price;
                    break;
                case 1://적군 경우에는 슬롯이 없으니까 인포를 고칠게 없을듯
                    break;
                case 2:
                    slotInfoList[id].price = GetOurForcesInfo(id, level).Price;
                    break;
                case 3:
                    slotInfoList[id].price = GetTrapInfo(id, level).Price;
                    break;
                case 4://기밀은 업그레이드 없음
                    //활성화 만 있음ㄴ
                    //slotInfoList[id].price = GetSecretInfo(id, level).Price;
                    //이건 플레이어 정보 수정에서 같이 동기화되야할꺼임
                    break;
                default:
                    break;
            }
        }
        catch (System.NullReferenceException) { Debug.Log("does not exist"); }

        SaveData();
    }

    void OnApplicationQuit()
    {
        SaveData();
    }
}
