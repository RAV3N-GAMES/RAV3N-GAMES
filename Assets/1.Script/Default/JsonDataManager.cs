using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using LitJson;

public class MyObject
{
    public string id;
    public int Lv;
    public int HP;
    public int Price;
    public int Upgrade;

    public MyObject(string id, int Lv, int HP, int Price, int Upgrade)
    {
        this.id = id;
        this.Lv = Lv;
        this.HP = HP;
        this.Price = Price;
        this.Upgrade = Upgrade;
    }
}


public class JsonDataManager : MonoBehaviour {
    public static Dictionary<string, SlotInfo> slotInfoList { get; private set; }   //현재까지 오브젝트 업데이트 정보및 슬롯관련 정보
                                                                                    //<id, SlotInfo>의 데이터 쌍
    public static Dictionary<string, Sprite> slotImage { get; private set; }

    public static List<MyObject> myObject { get; private set; }                     //GetObject 통해서 id, Lv에 맞는 오브젝트 찾도록


    void Awake()
    {
        slotInfoList = new Dictionary<string, SlotInfo>();
        slotImage = new Dictionary<string, Sprite>();
        myObject = new List<MyObject>();

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

    MyObject InitMyObject(JsonData data)
    {
        string id = data["id"].ToString();
        int Lv = int.Parse(data["Lv"].ToString());
        int HP = int.Parse(data["HP"].ToString());
        int Price = int.Parse(data["Price"].ToString());
        int Upgrade = int.Parse(data["Upgrade"].ToString());

        return new MyObject(id, Lv, HP, Price, Upgrade);
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
            {//임시
                newSprite = Resources.Load<GameObject>("Image/OldBuilding").GetComponent<SpriteRenderer>().sprite;
            }

            slotImage.Add(newSlot.imageName, newSprite);
        }
    }

    void LoadObjectInfoData()
    {
        foreach (var slot in slotInfoList)
        {
            string obj = File.ReadAllText(Application.dataPath + "/Resources/Data/ObjectInfo.json");
            JsonData objData = JsonMapper.ToObject(obj);

            for (int i = 0; i < objData.Count; i++)
            {
                MyObject myObj = InitMyObject(objData[i]);
                myObject.Add(myObj);
            }
        }
    }

    void LoadData()
    {
        LoadSlotData();
        //LoadObjectInfoData();
    }

    void SaveData()
    {
        List<SlotInfo> tmp = new List<SlotInfo>();
        
        foreach(var slot in slotInfoList)
        {
            tmp.Add(slot.Value);
        }

        JsonData newObj = JsonMapper.ToJson(tmp);
        File.WriteAllText(Application.dataPath + "/Resources/Data/SlotInfo.json", newObj.ToString());
    }

    public static MyObject GetObjectInfo(string id, int level)
    {
        for(int i = 0; i < myObject.Count; i++)
        {
            if(myObject[i].id == id)
            {
                if (myObject[i].Lv == level)
                    return myObject[i];
            }
        }

        return null;
    }

    void OnApplicationQuit()
    {
        SaveData();
    }
}
