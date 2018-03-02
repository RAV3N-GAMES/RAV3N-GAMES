using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using LitJson;

public class ResourceManager_Player : JsonReadWrite {
    private static int gold;
    [Range(1, 25)]
    private static int fame;
    private static int lvExperience;//레벨업에 필요한 경험치
    private static int experience;//플레이어가 현재 가지고 있는 경험치

    public static int Gold { get; set; }
    public static int Fame { get; set; }
    public static int LvExperience { get; set; }
    public static int Experience { get; set; }
    public static void addGold(int add_g) { gold += add_g; }
    public static void addFame(int add_f) { fame += add_f; }
    public static void subGold(int sub_g) { gold -= sub_g; }
    public static void subFame(int sub_f) { fame -= sub_f; }
    public static void addLvExperience(int add_le) { lvExperience+= add_le; }
    public static void addExperience(int add_e) {//player level이 최대치면 경험치 오르지 않음.
        if(fame<25)
            experience += add_e;
    }
    public static void subLvExperience(int sub_le) { lvExperience -= sub_le; }
    public static void subExperience(int sub_e) { experience -= sub_e; }
    public static bool isEnough_G(int price) { return (gold >= price) ? true : false; }

    public void fameObserve() {//update나 fixedupdate에서 실행. Experience에 따라 
        if (experience < 0)
        {
            fame--;
            experience = 0;
            //lvExperience=Json에서 읽어온 fame에 따른 필요 경험치.
        }
        else if (experience >= lvExperience) {
            
        }
    }
    /*
     * You can access the gold or fame by just using
     * ResourceManager_Player.Gold; or ResourceManager_Player.Fame;
     * If you want to set the gold or fame,
     * use ResourceManager_Player.Gold=10; or ResourceManager_Player.Fame=30;
     * Don't need to make an instance of ResourceManager_Player Just use it.
     * other static functions below are same.
     * like ResourceManager_Player.addGold(30);
     */

    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void ParsingJson(JsonData data)
    {
        Debug.Log("Please Override this function.");
    }

}
