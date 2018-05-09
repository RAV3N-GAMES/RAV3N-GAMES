using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warp_Enter : Trap {
    public GameObject newObj;
    public GameObject Effect;
    // Use this for initialization
    void Start () {
        //room Index 얻어올 방법?
        //checkCount(0);
        TrapManager.TrapCount[0]++;

        newObj = Instantiate(Resources.Load("Object/Warp_Exit") as GameObject);
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
        isCool = false;
    }

    public override void Acts(Collider col)
    {
        Effect.SetActive(true);
        Enemy e= col.GetComponentInParent<Enemy>();
        e.enemyAI.Warp(newObj.transform.position);
        StartCoroutine(CoolTimeDelay(CoolTime));
        
    }

}
