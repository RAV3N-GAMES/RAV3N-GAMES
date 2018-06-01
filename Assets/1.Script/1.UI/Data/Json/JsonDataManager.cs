using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using LitJson;


public class JsonDataManager : MonoBehaviour {
    public bool isSave;
    static bool isSaveForSave;
    public static Dictionary<string, SlotInfo> slotInfoList { get; private set; }   //현재까지 오브젝트 업데이트 정보및 슬롯관련 정보
                                                                                    //<id, SlotInfo>의 데이터 쌍
    public static Dictionary<string, Sprite> slotImage { get; private set; }
    public static Dictionary<string, Sprite> upgradeImage { get; private set; }
    public static Dictionary<string, Sprite> activationImage { get; private set; }

    public RoomManager roomManager;
    public List<SlotManager> slotManager;
    public MapManager mapManager;
    public MailBox mailBox;

    void Awake()
    {
        slotInfoList = new Dictionary<string, SlotInfo>();
        slotImage = new Dictionary<string, Sprite>();
        upgradeImage = new Dictionary<string, Sprite>();
        activationImage = new Dictionary<string, Sprite>();

        LoadData();
    }

    void Start()
    {
        isSaveForSave = isSave;

        Data_Player.addGold(100000);
        Data_Player.Fame = 4;
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
        string slotObj;
        print("LoadSlot");

        if (!isSave)
        {
            TextAsset textAsset = Resources.Load("Data/SlotInfo") as TextAsset;
            slotObj = textAsset.ToString();
        }
        else
        {
            try
            {
                slotObj = File.ReadAllText(Application.persistentDataPath + "/SlotInfo.json");
            }
            catch
            {
                TextAsset textAsset = Resources.Load("Data/SlotInfo") as TextAsset;
                slotObj = textAsset.ToString();
            }
        }

        JsonData slotData = JsonMapper.ToObject(slotObj);

        for (int i = 0; i < slotData.Count; i++)
        {
            SlotInfo newSlot = InitSlotInfo(slotData[i]);
            slotInfoList.Add(newSlot.id, newSlot);
        }
        if(!isSave)
        {
            SetSlotInfo(3, "Warp", 1);
            SetSlotInfo(3, "ObstructMovementCurrent", 1);
            SetSlotInfo(3, "FlameThrowingTrap", 1);
        }
        GameObject[] sprite = Resources.LoadAll<GameObject>("ChangePopUp");

        for(int i = 0; i < sprite.Length; i++)
        {
            slotImage.Add(sprite[i].name, sprite[i].GetComponent<SpriteRenderer>().sprite);
        }

        sprite = Resources.LoadAll<GameObject>("UpgradePopUp");

        for (int i = 0; i < sprite.Length; i++)
        {
            upgradeImage.Add(sprite[i].name, sprite[i].GetComponent<SpriteRenderer>().sprite);
        }

        sprite = Resources.LoadAll<GameObject>("ActivationPopUp");

        for (int i = 0; i < sprite.Length; i++)
        {
            activationImage.Add(sprite[i].name, sprite[i].GetComponent<SpriteRenderer>().sprite);
        }
    }
    

    void LoadData()
    {
        LoadSlotData();
    }

    static void SaveData()
    {
        if (isSaveForSave)
        {
            List<SlotInfo> tmp = new List<SlotInfo>();

            foreach (var slot in slotInfoList)
            {
                tmp.Add(slot.Value);
            }

            JsonData newObj = JsonMapper.ToJson(tmp);

            File.WriteAllText(Application.persistentDataPath + "/SlotInfo.json", newObj.ToString());
        }
    }
    
    public static BuildingObject GetBuildingInfo(string id, int level)
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

    public static EnemyObject GetEnemyInfo(string id, int level)
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

    public static OurForcesObject GetOurForcesInfo(string id, int level)
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

    public static SecretObject GetSecretInfo(string id, int fame)
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

    public static TrapObject GetTrapInfo(string id, int level)
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

        //0 : 건물, 1 : 적군, 2 : 아군, 3 : 함정, 4 : 기밀
        try
        {
            switch (Type)
            {
                case 0:
                    slotInfoList[id].price = GetBuildingInfo(id, level).Price;
                    break;
                case 2:
                    slotInfoList[id].price = GetOurForcesInfo(id, level).Price;
                    break;
                case 3:
                    slotInfoList[id].price = GetTrapInfo(id, level).Price;
                    break;
                default:
                    break;
            }
        }
        catch (System.NullReferenceException) { Debug.Log("does not exist"); }

        SaveData();
    }

    void ResetMapInfo()
    {
        TextAsset textAsset = Resources.Load("Data/MapInfo") as TextAsset;
        string mapData = textAsset.ToString();

        File.WriteAllText(Application.persistentDataPath + "/MapInfo.json", mapData);
    }

    void ResetSlotInfo()
    {
        TextAsset textAsset = Resources.Load("Data/SlotInfo") as TextAsset;
        string slotObj = textAsset.ToString();

        JsonData slotData = JsonMapper.ToObject(slotObj);

        slotInfoList.Clear();
        List<SlotInfo> slot = new List<SlotInfo>();

        for (int i = 0; i < slotData.Count; i++)
        {
            SlotInfo newSlot = InitSlotInfo(slotData[i]);

            slotInfoList.Add(newSlot.id, newSlot);
            slot.Add(newSlot);
        }

        JsonData newObj = JsonMapper.ToJson(slot);
        File.WriteAllText(Application.persistentDataPath + "/SlotInfo.json", newObj.ToString());
    }

    void ResetSaveObject()
    {
        TextAsset textAsset = Resources.Load("Data/Room" + 0) as TextAsset;
        string displayData = textAsset.ToString();

        File.WriteAllText(Application.persistentDataPath + "/Room0.json", displayData);

        for (int i = 1; i < 25; i++)
        {
            File.WriteAllText(Application.persistentDataPath + "/Room" + i + ".json", null);
        }
    }

    void ResetMailList()
    {
        List<int> tmp = new List<int>();
        JsonData newObj = JsonMapper.ToJson(tmp);

        File.WriteAllText(Application.persistentDataPath + "/MailList.json", newObj.ToString());
    }

    public void Reset()
    {
        //+플레이어 정보 초기화
        if (isSave)
        {
            ResetSlotInfo();
            for (int i = 0; i < slotManager.Count; i++)
                slotManager[i].RefreshInfo();

            ResetSaveObject();
            roomManager.ResetRoom();

            ResetMapInfo();
            mapManager.InitMapManager();

            ResetMailList();
            mailBox.RewardCoinList.Clear();
        }
    }
}
