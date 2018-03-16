using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySheriff : Enemy {

	private void Start()
	{
		effectType = EFFECT_TYPE.Hit;
		StopDistance = 2;
		attackDelay = new WaitForSeconds(2f);
	}
	public override void EnemyInit()
	{
		Hp = 100;
		Attack = 40;
		base.EnemyInit();

	}
}
