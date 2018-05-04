using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Trap : MonoBehaviour{
    public static int RoomMax=25;
    public Animator TrapAni;

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}   

    void OnTriggerEnter(Collider col) {
        if (col.CompareTag("EnemyBody"))
        {
            Acts(col);
        }
    }
    void OnTriggerExit(Collider col) {
    }
    public virtual void Acts(Collider col) { }
    public bool checkCount(int roomIdx) {//생성 전 Trap 개수와 TrapMax를 비교
        return (TrapManager.TrapMax == TrapManager.TrapCount[roomIdx]) ? true : false;
    }
}
