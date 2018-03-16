using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class SlotInfo : MonoBehaviour {
    public string id;

    public int price;

    public int level;
    public int HP;

    public Sprite slotImage;

    public void InitSlotInfo()
    {
        //전체 밸런스
        //내꺼 레벨 아이디 프라이스 등
        //오브젝트 위치
        level = 1;
        price = 1;
        HP = 1;
    }
}
