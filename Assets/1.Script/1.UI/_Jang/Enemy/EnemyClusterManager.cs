using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyClusterManager : MonoBehaviour {
    public static List<EnemyCluster> clusterList=new List<EnemyCluster>();
    public static bool IsStageEnd;
	// Use this for initialization
	void Awake () {
        IsStageEnd = false;
        EnemyCluster ec;
        for (int i = 0; i < ResourceManager_Player.ClusterMax; i++) {
            ec = gameObject.AddComponent<EnemyCluster>();
            clusterList.Add(ec);
        }
    }
	
	// Update is called once per frame
	void Update()
    {
        if (GameManager.GenerateComplete) {
            IsStageEnd = IsEnd();   
        }
    }

    public bool IsEnd() {
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
