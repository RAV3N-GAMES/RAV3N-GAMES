﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using LitJson;

/*
 * Tbl_SecretSetup Index
 * 0 ~ 21 : UFOCore
 * 22 ~ 43 : AlienStorageCapsule
 * 44 ~ 65 : AlienBloodStorage
 * 66 ~ 87 : SpaceVoiceRecordingFile
 */
public class SecretManager : JsonReadWrite
{
    public enum Group_Secret { UFOCore, AlienStorageCapsule, AlienBooldStorage, SpaceVoiceRecordingFile };
    private const int SecretLimit = 5;//Maximum is 5
    public static int SecretCount;//The number of existing secrets
    const string Path1 = "Data/Secret_UFOCore";
    const string Path2 = "Data/Secret_AlienStorageCapsule";
    const string Path3 = "Data/Secret_AlienBloodStorage";
    const string Path4 = "Data/Secret_SpaceVoiceRecordingFile";
    public static List<SecretObject> Tbl_SecretSetup = new List<SecretObject>();
    public static List<SecretActs> SecretList;
    public static Dictionary<string, int> SecretFame = new Dictionary<string, int>(); //첫 등장 Fame
    public static float criteria = 0.5f;//랜덤값이 이보다 크면 명예집단 / 작으면 기밀탈취집단.

    //    public static List<SecretActs> CurrentSecrets = new List<SecretActs>();//기밀이 1개 생성될 때마다 리스트에 추가. 다룰 일이 생기면 그때 넣도록.
    // Use this for initialization
    void Awake()
    {
        SecretCount = 0;
        ReadMain(Path1);
        ReadMain(Path2);
        ReadMain(Path3);
        ReadMain(Path4);
        SecretList = new List<SecretActs>();
        DontDestroyOnLoad(this);
        /*입력값 확인용 코드입니다. 지우지 마세요. 
                for (int i = 0; i < 22 * 4; i++)
                {
                    Debug.Log(i + "th index: " + Tbl_SecretSetup[i].Level + " " + Tbl_SecretSetup[i].Fame + " " + Tbl_SecretSetup[i].SecretBanditsGenChance + " " + Tbl_SecretSetup[i].Price+ " " + Tbl_SecretSetup[i].id+ " " + Tbl_SecretSetup[i].Type);
                }
        */
    }

    // Update is called once per frame
    void Update()
    {
    }

    public override void ParsingJson(JsonData data)
    {
        for (int i = 0; i < data.Count; i++)
        {
            SecretObject tmp = new SecretObject();
            tmp.Fame = int.Parse(data[i]["Fame"].ToString());
            tmp.Level = 1;
            tmp.SecretBanditsGenChance = double.Parse(data[i]["SecretBanditsGenChance"].ToString());
            tmp.Price = int.Parse(data[i]["Price"].ToString());
            tmp.id = data[i]["id"].ToString();
            tmp.Type = int.Parse(data[i]["Type"].ToString());
            Tbl_SecretSetup.Add(tmp);

            if (tmp.SecretBanditsGenChance != 0)
            {
                try
                {
                    int fame = SecretFame[tmp.id];
                    if (tmp.Fame < fame)
                        SecretFame[tmp.id] = tmp.Fame;
                }
                catch (KeyNotFoundException)
                {
                    SecretFame.Add(tmp.id, tmp.Fame);
                }
            }
        }
    }

    public static int GetSecretLimit()
    {
        return SecretLimit;
    }

    public static void CalCriteria(SecretActs sa){
        criteria += (float)sa.Chance;
    }
}