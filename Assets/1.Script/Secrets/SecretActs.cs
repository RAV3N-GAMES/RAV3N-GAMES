﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretActs : MonoBehaviour {
    /*
     * SecretManager.Tbl_SecretSetup를 가져와서 사용한다.
     * SecretManager.Tbl_SecretSetup는 List 형식의 Secret Setup 파일을 읽음.
     * SecretManager.Tbl_SecretSetup의 fame은 4부터 시작한다. 즉 Tbl_SecretSetup의 인덱스 +4가 fame이 된다.
     * 현재 플레이어의 명성에 따라 Chance를 변경한다.
     * Chance는 Wave 시작 시 적군 AI 생성을 맡은 스크립트에서 참조하여 적을 생성한다.
     */
    private float Chance;//기밀이 목적인 적 출현 확률
    public static bool secretSeizured;//기밀 탈취되었을 시 true => 이를 판단하는 스크립트 GameManager(가명)에서 판단

    // Use this for initialization
    void Start () {
        //현재 맵에 있는 Secret의 수를 1 늘린다. 최대치면 해당 gameObject를 삭제한다.
        secretSeizured = false;
        checkCount();
        if (Data_Player.Fame >= 4)
            initChance(this.name);
    }

    // Update is called once per frame
    void Update () {
		
	}

    public float getChance() { return Chance; }
    public void setChance(float chance) { Chance = chance; }

    private void checkCount() {
        if (SecretManager.SecretCount >= SecretManager.GetSecretLimit())
        {
            Destroy(this.gameObject);
            SecretManager.SecretCount -= 1;
            Debug.Log("Destroy the gameobject in SecretActs.cs Because of it's too much");
        }
        SecretManager.SecretCount += 1;
        Debug.Log("SecretCount: " + SecretManager.SecretCount);
    }
    private void initChance(string name) {
        switch (name)
        {
            case "UFOCore":
                Chance = SecretManager.Tbl_SecretSetup[Data_Player.Fame - 4].SecretSeizureChance_UFOCore;
                break;
            case "AlienBloodStorage":
                Chance = SecretManager.Tbl_SecretSetup[Data_Player.Fame - 4].SecretseizureChance_AlienBloodStorage;
                break;
            case "AlienStorageCapsule":
                Chance = SecretManager.Tbl_SecretSetup[Data_Player.Fame - 4].SecretSeizureChance_AlienStorageCapsule;
                break;
            case "SpaceVoiceRecordingFile":
                Chance = SecretManager.Tbl_SecretSetup[Data_Player.Fame - 4].SecretseizureChance_SpaceVoiceRecordingFile;
                break;
            default:
                Chance = SecretManager.Tbl_SecretSetup[Data_Player.Fame - 4].SecretSeizureChance_UFOCore;
                break;
        }
    }

    void onCollisionEnter(Collision col) {//tag가 Secret_Enemy인 적이 닿을 시 감점
        if (col.gameObject.tag == "Secret_Enemy") {
            secretSeizured = true;
        }
     }

}