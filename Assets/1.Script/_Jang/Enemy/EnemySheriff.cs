using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySheriff : Enemy {

	private void Start()
	{
		effectType = EFFECT_TYPE.Hit;
		attackDelay = new WaitForSeconds(2f);
	}
	public override void EnemyInit()
	{
        scollider.radius = (float)EnemyManager.Tbl_EnemySetup[Data_Player.Fame + 84].AttackRange * 2;
        StopDistance = scollider.radius;
        Hp = EnemyManager.Tbl_EnemySetup[Data_Player.Fame + 84].HP;
        Attack = EnemyManager.Tbl_EnemySetup[Data_Player.Fame + 84].Attack;
        MaxHp = Hp;
        base.EnemyInit();

	}
}
