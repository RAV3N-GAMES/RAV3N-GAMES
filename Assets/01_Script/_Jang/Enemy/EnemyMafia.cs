using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//
public class EnemyMafia : Enemy {

	private void Start()
	{
		effectType = EFFECT_TYPE.Hit;
		StopDistance = 2;
		attackDelay = new WaitForSeconds(1.2f);
	}
	public override void EnemyInit()
	{
		Hp = 100;
		MaxHp = Hp;
		Attack = 15;
		base.EnemyInit();
	}
}
