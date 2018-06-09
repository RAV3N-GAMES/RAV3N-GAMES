﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObjectType
{
    Building,
    Enemy,
    Friendly,
    Trap,
    Secret
}

//Data_SetupPlayer : 플레이어의 명성에 따른 레벨업 필요 경험치 및 집단 등장 수, 적 사살 시 보상, 적 집단 제압 시 보상을 저장.0
public class Data_SetupPlayer
{
    public int Fame;
    public int Reward_Min;//해당 Fame 최소 경험치
    public int Reward_Max;//해당 Fame 최대 경험치
    public int enemyClusterNumber;//출현하는 적 그룹 개수
    public int Reward_Kill;//적 1킬 당 보상 골드
    public int Reward_GroupOppression;//적 그룹 제압 보상 골드

    public Data_SetupPlayer()
    {
        Fame = -1;
        Reward_Min = -1;
        Reward_Max = -1;
        enemyClusterNumber = -1;
        Reward_Kill = -1;
        Reward_GroupOppression = -1;
    }
}

/*
 * Data_Player 클래스
 *  - stores Player information
 *      - fame
 *      - lvExperience
 *      - experience
 *      - get/set and add/sub methods of each above 3 properties.
 *      - isEnough method to check is the gold enough to buy an object or an upgrade
 *      - fameObserve for observing the change of fame.
 */
public static class Data_Player
{
    /*
     * Building ~ SecretTypes
     *  - 각 항목 별로 존재하는 종류 수
     *  ex) Building: Old, New, Functional, Core의 4종류
     *  OurForces : Guard, Quick~, Chemistry, Researcher의 4종류
     *  ...
     */

    const int BuildingTypes = 4;
    const int OurForcesTypes = 4;
    const int TrapTypes = 4;

    public static int[] BuildingLv = new int[BuildingTypes];
    public static int[] OurForcesLv = new int[OurForcesTypes];
    public static int[] TrapLv = new int[TrapTypes];

    public static int Gold;
    [Range(1, 25)]
    public static int Fame;
    public static int LvExperience { get; set; }//이 값 보다 Experience가 크면 레벨 업
    public static int LvExperienceDown { get; set; }//이 값 보다 Experience가 작으면 레벨 다운
    public static int Experience { get; set; }//현재 경험치(현상금)
    public static void addGold(int add_g) {
        Gold += add_g;
        try
        {
            if (add_g != 0)
                SoundManager.soundManager.OnEffectSound("9_GOLD");
        }
        catch (System.NullReferenceException) {; }
    }
    public static void addFame(int add_f) { Fame += add_f; MapManager.mapManager.ChangedFame(); }
    public static void subGold(int sub_g) {
        Gold -= sub_g;
        try
        {
            if (sub_g != 0)
                SoundManager.soundManager.OnEffectSound("9_GOLD");
        }
        catch (System.NullReferenceException) { ; }
    }
    public static void subFame(int sub_f) { Fame -= sub_f; MapManager.mapManager.ChangedFame(); }
    public static void addLvExperience(int add_le) { LvExperience += add_le; }
    public static void addExperience(int add_e)
    {//player level이 최대치면 경험치 오르지 않음.
        if (Fame < 25)
            Experience += add_e;
    }
    public static void subLvExperience(int sub_le) { LvExperience -= sub_le; }
    public static void subExperience(int sub_e) { Experience -= sub_e; }
    public static bool isEnough_G(int price) { return (Gold >= price) ? true : false; }

    public static void initObjectLv()
    {//빌딩, 아군, 함정의 레벨 상태 초기화
        int i = 0;
        for (i = 0; i < BuildingTypes; i++)
            BuildingLv[i] = 1;
        for (i = 0; i < OurForcesTypes; i++)
            OurForcesLv[i] = 1;
        for (i = 0; i < TrapTypes; i++)
            TrapLv[i] = 1;
    }

    public static void SetLvExperiences() {
        if (Fame >= 4) {
            LvExperienceDown = ResourceManager_Player.Tbl_Player[Fame - 4].Reward_Min;
            LvExperience = ResourceManager_Player.Tbl_Player[Fame - 4].Reward_Max;
        }
    }
    
    public static void LvUP() {
        Fame++;
        SetLvExperiences();
    }

    public static void LvDown() {
        Fame--;
        SetLvExperiences();
    }

    public static void SetLv(int wantedLv) {
        Fame = wantedLv;
        SetLvExperiences();
    }
    /*
     * 차후 구현
     * fame의 변화를 감지할 수 있어야 함
     * 4 => 5(1개 방 추가 개방) 이후
     * 5 => 4 => 5 순으로 fame이 오를 시 두 번째로 fame이 5가 되는 순간은 방 개방 X
     * 이를 감지할 수 있어야 함
     *  -> 1. 파일에 최대 진행한 fame을 써놓고 이보다 fame이 올라갈 경우에만 방/시설(벽, 함정, 기밀 등)을 개방한다.
     *     2. 전략의 하나로 묵인하고 처리하지 않는다.
     * 물어봅시다
    */

    /*
     * You can access the gold or fame by just using
     * Data_Player.Gold; or Data_Player.Fame;
     * If you want to set the gold or fame,
     * use Data_Player.Gold=10; or Data_Player.Fame=30;
     * Don't need to make an instance of Data_Player. Just use it.
     * other static functions below are same.
     * like Data_Player.addGold(30);
     */
}