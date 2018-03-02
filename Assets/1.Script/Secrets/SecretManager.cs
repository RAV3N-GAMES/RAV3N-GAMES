using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using LitJson;

//Data : 기밀의 레벨에 따른 매혹도 저장
[Serializable]
public class Data{
    public int fame;
    public float SecretSeizureChance_UfoCore, SecretSeizureChance_AlienStorageCapsule, SecretseizureChance_AlienBloodArchive, SecretseizureChance_AlienVoiceRecordingFile;
    public int[] Price=new int[4];
    public Data() {
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

public class SecretManager : MonoBehaviour {
    public enum Group_Secret { UFOCore, AlienStorageCapsule, AlienBloodArchive, SecretseizureChance_AlienVoiceRecordingFile };
    private string OriginalFile;
    private const int SecretLimit=5;//Maximum is 5
    public static int SecretCount;//The number of existing secrets

    const string OriginalFilePath = "./Assets/1.Script/Secrets/SecretDetails.json";
    public static List<Data> Tbl = new List<Data>();

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
        JsonData data = JsonMapper.ToObject(File);
        ParsingJson(data);
        yield return null;
    }

    private void ParsingJson(JsonData data){
        for (int i = 0; i < data.Count; i++){
            Data tmp = new Data();
            tmp.fame = int.Parse(data[i]["fame"].ToString());
            tmp.SecretSeizureChance_UfoCore = float.Parse(data[i]["SecretSeizureChance_UfoCore"].ToString());
            tmp.SecretSeizureChance_AlienStorageCapsule = float.Parse(data[i]["SecretSeizureChance_AlienStorageCapsule"].ToString());
            tmp.SecretseizureChance_AlienBloodArchive = float.Parse(data[i]["SecretseizureChance_AlienBloodArchive"].ToString());
            tmp.SecretseizureChance_AlienVoiceRecordingFile = float.Parse(data[i]["SecretseizureChance_AlienVoiceRecordingFile"].ToString());
            Tbl.Add(tmp);
        }
    }

    public static int GetSecretLimit() {
        return SecretLimit;
    }
}