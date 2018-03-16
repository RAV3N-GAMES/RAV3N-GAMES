using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFlyTeen : Enemy
{
	private void Start()
	{
		StopDistance = 1;
		effectType = EFFECT_TYPE.Approach;
		attackDelay = new WaitForSeconds(1f);
	}
	public override void EnemyInit()
	{
		Hp = 200;
		Attack = 5;
		base.EnemyInit();
	}
}
