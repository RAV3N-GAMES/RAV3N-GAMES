using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warp_Enter : Trap {
    public Warp_Exit WarpExit;
    public GameObject Effect;
    // Use this for initialization
    void Start () {
        TrapManager.TrapCount[0]++;
        CoolTime = TrapManager.Tbl_TrapSetup[Data_Player.Fame +18].CoolTime;
        EffectContinuousTime = TrapManager.Tbl_TrapSetup[Data_Player.Fame +18].EffectContinuousTime;
        isCool = false;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator CoolTimeDelay(double CoolTime)
    {
        isCool = true;
        yield return new WaitForSeconds((float)CoolTime);
        Effect.SetActive(false);
        WarpExit.Effect.SetActive(false);
        isCool = false;
    }

    public override void Acts(Collider col)
    {
        Effect.SetActive(true);
        WarpExit.Effect.SetActive(true);
        Enemy e= col.GetComponentInParent<Enemy>();
        e.enemyAI.Warp(WarpExit.transform.position);
        StartCoroutine(CoolTimeDelay(CoolTime));
    }
}