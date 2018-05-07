using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCluster : MonoBehaviour {
    public List<Enemy> eList;

	// Use this for initialization
	void Awake () {
        eList = new List<Enemy>();
    }

    // Update is called once per frame
    void Update () {
		
	}

    public void clusterSetNextIdx(int idx)
    {
        for (int i = 0; i < eList.Count; i++) {
            eList[i].nextIdx = idx;
        }
    }

    public void clusterSetNextTargetFriend(Friendly f) {
        for (int i = 0; i < eList.Count; i++)
        {
            eList[i].targetFriend= f;
        }
    }

    public void clusterSetNextTargetSecret(SecretActs s) {
        for (int i = 0; i < eList.Count; i++)
        {
            eList[i].targetSecret = s;
        }
    }

    public void clusterSetNextTargetTrap(Trap t) {
        for (int i = 0; i < eList.Count; i++)
        {
            eList[i].targetTrap = t;
        }
    }

    public void clusterSetNextTargetWall(Wall w) {
        for (int i = 0; i < eList.Count; i++)
        {
            eList[i].targetWall = w;
        }
    }

    public void clusterStole(SecretActs s) {
        if(eList.Count>0)
            StartCoroutine(eList[0].StealEvent());
    }

    public void clusterEscape() {
        for (int i = 0; i < eList.Count; i++)
        {
            StartCoroutine(eList[i].EscapeEvent());
        }
    }

    public Enemy HurtEnemy() {
        if (eList.Count <= 0)
            return null;
        Enemy e=eList[0];
        for (int i = 1; i < eList.Count; i++) {
            if (e.Hp > eList[i].Hp) {
                e = eList[i];
            }
        }
        
        if (e.Hp >= e.MaxHp)//모두 만피면 그냥 갈길 가도록 null return
            return null;
        else 
            return e;
    }
}