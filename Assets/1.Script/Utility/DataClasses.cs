using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Data : 기밀의 레벨에 따른 매혹도 저장
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
    public static void addLvExperience(int add_le) { lvExperience += add_le; }
    public static void addExperience(int add_e)
    {//player level이 최대치면 경험치 오르지 않음.
        if (fame < 25)
            experience += add_e;
    }
    public static void subLvExperience(int sub_le) { lvExperience -= sub_le; }
    public static void subExperience(int sub_e) { experience -= sub_e; }
    public static bool isEnough_G(int price) { return (gold >= price) ? true : false; }

    public static void fameObserve()
    {//update나 fixedupdate에서 실행. Experience에 따라 
        if (experience < 0)
        {
            fame--;
            experience = 0;
            //lvExperience=Json에서 읽어온 fame에 따른 필요 경험치.
        }
        else if (experience >= lvExperience)
        {

        }
    }
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

//Data_Secret : 기밀의 레벨에 따른 매혹도 저장
public class Data_Secret
{
    public int fame;
    public float SecretSeizureChance_UfoCore, SecretSeizureChance_AlienStorageCapsule, SecretseizureChance_AlienBloodArchive, SecretseizureChance_AlienVoiceRecordingFile;
    public int[] Price = new int[4];
    public Data_Secret()
    {
        fame = -1;
        SecretSeizureChance_UfoCore = -1;
        SecretSeizureChance_AlienStorageCapsule = -1;
        SecretseizureChance_AlienBloodArchive = -1;
        SecretseizureChance_AlienVoiceRecordingFile = -1;

        Price[(int)SecretManager.Group_Secret.UFOCore] = 5000;
        Price[(int)SecretManager.Group_Secret.AlienStorageCapsule] = 10000;
        Price[(int)SecretManager.Group_Secret.AlienBloodArchive] = 15000;
        Price[(int)SecretManager.Group_Secret.SecretseizureChance_AlienVoiceRecordingFile] = 20000;
    }
}