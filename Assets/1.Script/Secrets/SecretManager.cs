using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using LitJson;

public class SecretManager : JsonReadWrite {
    public enum Group_Secret { UFOCore, AlienStorageCapsule, AlienBooldStorage, SpaceVoiceRecordingFile };
    private const int SecretLimit=5;//Maximum is 5
    public static int SecretCount;//The number of existing secrets
    const string OriginalFilePath = "./Assets/Resources/Data/SecretDetails.json";
    public static List<Data_Secret> Tbl_SecretSetup = new List<Data_Secret>();
//    public static List<SecretActs> CurrentSecrets = new List<SecretActs>();//기밀이 1개 생성될 때마다 리스트에 추가. 다룰 일이 생기면 그때 넣도록.
    // Use this for initialization
    void Awake () {
        ReadMain(OriginalFilePath);
        //Debug.Log("SecretManager data[0]: " + Tbl_SecretSetup[0].fame + " " + Tbl_SecretSetup[0].SecretSeizureChance_UFOCore + " " + " " + Tbl_SecretSetup[0].SecretSeizureChance_AlienStorageCapsule + " " + Tbl_SecretSetup[0].SecretseizureChance_AlienBloodStorage + " " + Tbl_SecretSetup[0].SecretseizureChance_SpaceVoiceRecordingFile);
        //Debug.Log("SecretManager data[10]: " + Tbl_SecretSetup[10].fame + " " + Tbl_SecretSetup[10].SecretSeizureChance_UFOCore + " " + " " + Tbl_SecretSetup[10].SecretSeizureChance_AlienStorageCapsule + " " + Tbl_SecretSetup[10].SecretseizureChance_AlienBloodStorage + " " + Tbl_SecretSetup[10].SecretseizureChance_SpaceVoiceRecordingFile);
        //Debug.Log("SecretManager data[20]: " + Tbl_SecretSetup[20].fame + " " + Tbl_SecretSetup[20].SecretSeizureChance_UFOCore + " " + " " + Tbl_SecretSetup[20].SecretSeizureChance_AlienStorageCapsule + " " + Tbl_SecretSetup[20].SecretseizureChance_AlienBloodStorage + " " + Tbl_SecretSetup[20].SecretseizureChance_SpaceVoiceRecordingFile);
    }
	
	// Update is called once per frame
	void Update () {
    }

    
    public override void ParsingJson(JsonData Data_Secret){
        for (int i = 0; i < Data_Secret.Count; i++){
            Data_Secret tmp = new Data_Secret();
            tmp.fame = int.Parse(Data_Secret[i]["fame"].ToString());
            tmp.SecretSeizureChance_UFOCore = float.Parse(Data_Secret[i]["SecretSeizureChance_UFOCore"].ToString());
            tmp.SecretSeizureChance_AlienStorageCapsule = float.Parse(Data_Secret[i]["SecretSeizureChance_AlienStorageCapsule"].ToString());
            tmp.SecretseizureChance_AlienBloodStorage = float.Parse(Data_Secret[i]["SecretseizureChance_AlienBloodStorage"].ToString());
            tmp.SecretseizureChance_SpaceVoiceRecordingFile = float.Parse(Data_Secret[i]["SecretseizureChance_SpaceVoiceRecordingFile"].ToString());
            Tbl_SecretSetup.Add(tmp);
        }
    }

    public static int GetSecretLimit() {
        return SecretLimit;
    }
}