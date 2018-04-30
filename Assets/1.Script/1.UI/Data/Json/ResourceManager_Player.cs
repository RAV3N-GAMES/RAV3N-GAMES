using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using LitJson;

public class ResourceManager_Player : JsonReadWrite
{
    private const string Path = "Data/PlayerDetail";
    public static List<Data_SetupPlayer> Tbl_Player = new List<Data_SetupPlayer>();
    //    public enum Group_Palyer { fame, lvExperience, enemyClusterNumber, rewardA, rewardB};

    void Awake()
    {
        ReadMain(Path);
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void ParsingJson(JsonData data)
    {
        for (int i = 0; i < data.Count; i++)
        {
            Data_SetupPlayer tmp = new Data_SetupPlayer();
            tmp.Fame = int.Parse(data[i]["Fame"].ToString());
            tmp.Reward_Min= int.Parse(data[i]["Reward_Min"].ToString());
            tmp.Reward_Max= int.Parse(data[i]["Reward_Max"].ToString());
            tmp.enemyClusterNumber = int.Parse(data[i]["enemyClusterNumber"].ToString());
            tmp.Reward_Kill = int.Parse(data[i]["Reward_Kill"].ToString());
            tmp.Reward_GroupOppression = int.Parse(data[i]["Reward_GroupOppression"].ToString());
            Tbl_Player.Add(tmp);
        }
    }

    public static int GetPlayerFame(int Exp)
    {
        if (Exp < 0)
            return 0;

        for(int i = 0; i < Tbl_Player.Count; i++)
        {
            if(Tbl_Player[i].Reward_Min > Exp)
            {
                return i + 3;
            }
        }

        return 25;
    }
}