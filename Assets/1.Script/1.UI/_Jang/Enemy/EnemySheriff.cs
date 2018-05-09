using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySheriff : Enemy {
    private void Start()
	{
        Clips = Resources.LoadAll<AudioClip>(path + "Enemy/Sheriff") as AudioClip[];
        DieClip = Resources.LoadAll<AudioClip>(path + "Man") as AudioClip[];
        effectType = EFFECT_TYPE.Hit;
		attackDelay = new WaitForSeconds(2f);
	}
	public override void EnemyInit()
	{
        scollider.radius = (float)EnemyManager.Tbl_EnemySetup[Data_Player.Fame + 84].AttackRange * 2;
        Hp = EnemyManager.Tbl_EnemySetup[Data_Player.Fame + 84].HP;
        Attack = EnemyManager.Tbl_EnemySetup[Data_Player.Fame + 84].Attack;
        MaxHp = Hp;
        isHealer = false;
        base.EnemyInit();
        enemyAI.stoppingDistance = scollider.radius;
    }
}
