using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//
public class EnemyMafia : Enemy {

	private void Start()
	{
		effectType = EFFECT_TYPE.Hit;
		attackDelay = new WaitForSeconds(1.2f);
	}
	public override void EnemyInit()
	{
        scollider.radius = (float)EnemyManager.Tbl_EnemySetup[Data_Player.Fame + 18].AttackRange * 2;
        Hp = EnemyManager.Tbl_EnemySetup[Data_Player.Fame + 18].HP;
        Attack = EnemyManager.Tbl_EnemySetup[Data_Player.Fame + 18].Attack;
		MaxHp = Hp;
        isHealer = false;
		base.EnemyInit();
        enemyAI.stoppingDistance = scollider.radius;
	}
}
