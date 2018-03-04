using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using LitJson;

public class SecretManager : MonoBehaviour {
    public enum Group_Secret { UFOCore, AlienStorageCapsule, AlienBloodArchive, SecretseizureChance_AlienVoiceRecordingFile };
    private string OriginalFile;
    private const int SecretLimit=5;//Maximum is 5
    public static int SecretCount;//The number of existing secrets

    const string OriginalFilePath = "./Assets/1.Script/Secrets/SecretDetails.json";
    public static List<Data_Secret> Tbl = new List<Data_Secret>();

    // Use this for initialization
    void Awake () {
        OriginalFile = ReadFile(OriginalFilePath);
        JsonParse(OriginalFile);
    }
	
	// Update is called once per frame
	void Update () {
    }

    //설정파일(SecretDetails.json)을 읽어옴
    string ReadFile(string Path){
        return File.ReadAllText(Path);
    }
    void JsonParse(string File){
        StartCoroutine(LoadCo(File));
    }

    IEnumerator LoadCo(string File){
        JsonData Data_Secret = JsonMapper.ToObject(File);
        ParsingJson(Data_Secret);
        yield return null;
    }

    private void ParsingJson(JsonData Data_Secret){
        for (int i = 0; i < Data_Secret.Count; i++){
            Data_Secret tmp = new Data_Secret();
            tmp.fame = int.Parse(Data_Secret[i]["fame"].ToString());
            tmp.SecretSeizureChance_UfoCore = float.Parse(Data_Secret[i]["SecretSeizureChance_UfoCore"].ToString());
            tmp.SecretSeizureChance_AlienStorageCapsule = float.Parse(Data_Secret[i]["SecretSeizureChance_AlienStorageCapsule"].ToString());
            tmp.SecretseizureChance_AlienBloodArchive = float.Parse(Data_Secret[i]["SecretseizureChance_AlienBloodArchive"].ToString());
            tmp.SecretseizureChance_AlienVoiceRecordingFile = float.Parse(Data_Secret[i]["SecretseizureChance_AlienVoiceRecordingFile"].ToString());
            Tbl.Add(tmp);
        }
    }

    public static int GetSecretLimit() {
        return SecretLimit;
    }
}