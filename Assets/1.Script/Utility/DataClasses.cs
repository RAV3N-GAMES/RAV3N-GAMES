using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Data_SetupPlayer : 플레이어의 명성에 따른 레벨업 필요 경험치 및 집단 등장 수, 적 사살 시 보상, 적 집단 제압 시 보상을 저장.0
public class Data_SetupPlayer
{
    public int fame, lvExperience, enemyClusterNumber, rewardA, rewardB;

    public Data_SetupPlayer()
    {
        fame = -1;
        lvExperience = -1;
        enemyClusterNumber = -1;
        rewardA = -1;
        rewardB = -1;
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

    const int BuildingTypes=4;
    const int OurForcesTypes = 4;
    const int TrapTypes = 4;

    private static int gold;
    [Range(1, 25)]
    private static int fame;
    private static int lvExperience;//레벨업에 필요한 경험치
    private static int experience;//플레이어가 현재 가지고 있는 경험치
    public static int []BuildingLv=new int [BuildingTypes];
    public static int[] OurForcesLv = new int[OurForcesTypes];
    public static int[] TrapLv = new int[TrapTypes];

    public static int Gold { get; set; }
    public static int Fame { get; set; }
    public static int LvExperience { get; set; }
    public static int Experience { get; set; }
    public static void addGold(int add_g) { gold += add_g; }
    public static void addFame(int add_f) { fame += add_f; }
    public static void subGold(int sub_g) { gold -= sub_g; }
    public static void subFame(int sub_f) { fame -= sub_f; }
    public static void addLvExperience(int add_le) { lvExperience += add_le; }
    public static void addExperience(int add_e)
    {//player level이 최대치면 경험치 오르지 않음.
        if (fame < 25)
            experience += add_e;
    }
    public static void subLvExperience(int sub_le) { lvExperience -= sub_le; }
    public static void subExperience(int sub_e) { experience -= sub_e; }
    public static bool isEnough_G(int price) { return (gold >= price) ? true : false; }

    public static void initObjectLv() {//빌딩, 아군, 함정의 레벨 상태 초기화
        int i = 0;
        for (i = 0; i < BuildingTypes; i++)
            BuildingLv[i] = 1;
        for (i = 0; i < OurForcesTypes; i++)
            OurForcesLv[i] = 1;
        for (i = 0; i < TrapTypes; i++)
            TrapLv[i] = 1;
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