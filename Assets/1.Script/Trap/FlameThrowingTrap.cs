using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameThrowingTrap : Trap {
    double EffectContinuousTime;
    public double CoolTime;
    public bool isCool;
    public int AttackDamage;
    List<Enemy> EnemyList;

    // Use this for initialization
    void Start () {
        EnemyList = new List<Enemy>();
        CoolTime = TrapManager.Tbl_TrapSetup[Data_Player.Fame + 40].CoolTime;
        EffectContinuousTime = TrapManager.Tbl_TrapSetup[Data_Player.Fame + 40].EffectContinuousTime;
        AttackDamage=TrapManager.Tbl_TrapSetup[Data_Player.Fame + 40].Attack;
        isCool = false;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    IEnumerator CoolTimeDelay(double CoolTime)
    {
        isCool = true;
        yield return new WaitForSeconds((float)CoolTime);
        isCool = false;
    }

    public override void Acts(Collider col)
    {
        /*
         * 화염방사 함정 
         *  - 1. 상시 화염방사
         *  - 2. collider에 enemy가 닿으면 데미지
         */
        if (!isCool)
        {
            Enemy targetEnemy=col.GetComponent<Enemy>();
            
            if (targetEnemy.Health(AttackDamage))
            {
                targetEnemy.Die();
                targetEnemy = targetEnemy.GroupConductor.GroupFindEnemy();
            }
            StartCoroutine(CoolTimeDelay(CoolTime));
        }
    }
}