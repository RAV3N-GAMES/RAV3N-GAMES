using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySinger : Enemy {

	WaitForSeconds healDelay = new WaitForSeconds(2f);

	Enemy healTarget = null;

	private bool isHeal = false;
	private void Start()
	{
		EnemyType = ENEMY_TYPE.Sheriff;
		effectType = EFFECT_TYPE.Heal;
		StopDistance = 2;
		isDie = false;
	}
	public override void EnemyInit()
	{
		Hp = 100;
		Attack = 30;
		base.EnemyInit();
	}

	private void Update()
	{
		if (isDie)
			return;
		
		if (healTarget != GroupConductor.GetLessHpEnemy(this))
			healTarget = GroupConductor.GetLessHpEnemy(this);

		if (healTarget != null)
		{
			if (HealingDirDisatnce() == true)
			{
				currentState = EnemyState.Idle;
				if(!isHeal)
				{
					isHeal = true;
					StartCoroutine(GiveHeal());
				}
			}
			else
			{
				currentState = EnemyState.Walk;
				enemyAI.SetDestination(healTarget.NavObj.position);
			}
		}

		ChangeAnimation();
	}
	private void OriginalDest()
	{
		Target = OriginalPoint;
		dest = new Vector3(OriginalPoint.position.x, 0, OriginalPoint.position.z);
		start = new Vector3(NavObj.position.x, 0, NavObj.position.z);
		Distance = Vector3.Distance(start, dest);

		if (Distance < StopDistance)
		{
			if (enemyAI.enabled)
			{
				enemyAI.enabled = false;
			}
			currentState = EnemyState.Idle;
		}
		else if (Distance > StopDistance)
		{
	
			if (!enemyAI.enabled)
			{
				enemyAI.enabled = true;
			}
			currentState = EnemyState.Walk;
			enemyAI.SetDestination(OriginalPoint.position);
		}

	}
	private bool HealingDirDisatnce()
	{
		dest = new Vector3(healTarget.NavObj.position.x, 0, healTarget.NavObj.position.z);
		start = new Vector3(NavObj.position.x, 0, NavObj.position.z);
		Distance = Vector3.Distance(dest, start);

		if (healTarget.NavObj.position.x > NavObj.position.x)
			transform.localScale = new Vector3(-1, 1, 1);
		else
			transform.localScale = new Vector3(1, 1, 1);
		if (Distance < StopDistance)
			return true;
		else
			return false;
	}

	IEnumerator GiveHeal()
	{
		if (healTarget == null || healTarget == this)
			yield break;
		
		healTarget.Hp += Attack;
		healTarget.UIEnemyHealth.ValueIncrease(Attack);
		GameManager.ParticleGenerate(effectType,
			healTarget.NavObj.position);
		
		yield return healDelay;

		isHeal = false;
	}
	

}
