using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyClusterManager : MonoBehaviour {
    public static List<EnemyCluster> clusterList=new List<EnemyCluster>();
    public DayandNight Curtain;
    // Use this for initialization
	void Awake () {
        EnemyCluster ec;
        for (int i = 0; i < ResourceManager_Player.ClusterMax; i++) {
            ec = gameObject.AddComponent<EnemyCluster>();
            clusterList.Add(ec);
            ec.myIdx = i;
        }
    }

    void Update() {
        if (GameManager.GenerateComplete && DayandNight.isDay) {
            bool IsEnd=CheckEnd();
            if (IsEnd)
            {
                try
                {
                    Curtain= GameObject.Find("Canvas").GetComponentInChildren<DayandNight>();
                    if (Curtain) {
                        GameManager.GenerateComplete = false;
                        Curtain.TurnToNight();
                    }
                }
                catch { }
            }
        }
    }
    
    public bool CheckEnd() {
        bool isEnd = true;
        EnemyCluster ec;
        for (int i = 0; (i < clusterList.Count) && isEnd; i++)
        {
            ec = clusterList[i];
            for (int j = 0; j < ec.eList.Count; i++) {
                if (ec.eList[j].gameObject.activeSelf) {
                    isEnd = false;
                    break;
                }
            }
        }
        return isEnd;
    }

    public void EscapeClusters() {
        for (int i = 0; i < clusterList.Count; i++) {
            clusterList[i].clusterEscape();
        }
    }
}
