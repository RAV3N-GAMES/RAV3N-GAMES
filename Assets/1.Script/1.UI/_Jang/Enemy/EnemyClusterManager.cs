using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyClusterManager : MonoBehaviour {
    public static List<EnemyCluster> clusterList=new List<EnemyCluster>();
	// Use this for initialization
	void Awake () {
        EnemyCluster ec;
        for (int i = 0; i < ResourceManager_Player.ClusterMax; i++) {
            ec = gameObject.AddComponent<EnemyCluster>();
            clusterList.Add(ec);
        }
        DontDestroyOnLoad(this);
    }
	
	// Update is called once per frame
	void Update () {	
	}

    public void EscapeClusters() {
        for (int i = 0; i < clusterList.Count; i++) {
            clusterList[i].clusterEscape();
        }
    }
}
