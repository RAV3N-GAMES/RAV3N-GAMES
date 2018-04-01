using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class SlotInfo {
    //0 : 건물
    //1 : 적군
    //2 : 아군
    //3 : 함정
    //4 : 기밀
    public int type;

    public string id;
    public int price;
    public int level;
    public string imageName;

    public SlotInfo(int type,string id, int price, int level, string imageName)
    {
        //level에 따라 따로 저장된 JsonData 불러와야함
        this.type = type;
        this.id = id;
        this.price = price;
        this.level = level;
        this.imageName = imageName;
    }
}
