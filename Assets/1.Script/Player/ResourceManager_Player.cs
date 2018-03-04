using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using LitJson;

public class ResourceManager_Player : JsonReadWrite {
    
    private const string Path = "./Assets/1.Script/Player/PlayerDetail.json";
    private static List<Data_SetupPlayer> Tbl_Player = new List<Data_SetupPlayer>();
//    public enum Group_Palyer { fame, lvExperience, enemyClusterNumber, rewardA, rewardB};

    void Awake() {
        ReadMain(Path);
        Debug.Log(Tbl_Player[0].fame+" "+Tbl_Player[0].lvExperience);
        Debug.Log(Tbl_Player[10].fame + " " + Tbl_Player[10].lvExperience);
    }

    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void ParsingJson(JsonData data)
    {
        for (int i = 0; i < data.Count; i++) {
            Data_SetupPlayer tmp = new Data_SetupPlayer();
            tmp.fame = int.Parse(data[i]["fame"].ToString());
            tmp.lvExperience = int.Parse(data[i]["lvExperience"].ToString());
            tmp.enemyClusterNumber = int.Parse(data[i]["enemyClusterNumber"].ToString());
            tmp.rewardA = int.Parse(data[i]["rewardA"].ToString());
            tmp.rewardB = int.Parse(data[i]["rewardB"].ToString());
            Tbl_Player.Add(tmp);
        }
    }

}
