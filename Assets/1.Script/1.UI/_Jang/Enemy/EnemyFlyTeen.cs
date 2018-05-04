using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFlyTeen : Enemy
{
    private void Start()
	{
		effectType = EFFECT_TYPE.Approach;
		attackDelay = new WaitForSeconds(1f);
	}
	public override void EnemyInit()
	{
        scollider.radius = (float)EnemyManager.Tbl_EnemySetup[Data_Player.Fame-4].AttackRange*6;
        Hp = EnemyManager.Tbl_EnemySetup[Data_Player.Fame-4].HP;
		Attack = EnemyManager.Tbl_EnemySetup[Data_Player.Fame-4].Attack;
        MaxHp = Hp;
        isHealer = false;
		base.EnemyInit();
        enemyAI.stoppingDistance = 0.5f;

    }
}
