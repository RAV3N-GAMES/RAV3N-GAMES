using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Trap : MonoBehaviour{
    public AudioSource Audio;
    public static int RoomMax=25;
    public Animator TrapAni;
    public double EffectContinuousTime;
    public double CoolTime;
    public bool isCool;
    public int AttackDamage;
    public int setRoomidx;
    public DisplayObject displayobject;
    // Use this for initialization
    void Awake () {
        Audio = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}   

    void OnTriggerEnter(Collider col) {
        if (col.CompareTag("EnemyBody"))
        {
            if(!isCool)
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
