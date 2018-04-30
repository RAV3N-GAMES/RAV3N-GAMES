using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveEnemyManager : MonoBehaviour {
    public static bool IsCreated;
    private bool ChangeLock;
    public DayandNight Curtain;

	// Use this for initialization
	void Awake () {
        IsCreated = false;
        ChangeLock = false;

    }
	
	// Update is called once per frame
	void Update () {
        if (DayandNight.isDay && IsCreated) {
            GameObject[] ActiveEnemyList = GameObject.FindGameObjectsWithTag("Enemy");

            if (ActiveEnemyList.Length > 0)
                ChangeLock = true;
            
            if (ChangeLock && ActiveEnemyList.Length <= 0) {
                IsCreated = false;
                ChangeLock = false;
                Curtain.changeState();
            }
        }
	}
}
