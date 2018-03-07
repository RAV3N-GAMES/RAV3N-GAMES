using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendlyResearcher : Friendly {

	bool isHeal = false;
	WaitForSeconds healDelay = new WaitForSeconds(2f);
	Friendly healTarget = null;



	private void Start()
	{
		Hp = 120;
		AttackDamage = 22;
		FriendType = FRIEND_TYPE.Resercher;
		effectType = EFFECT_TYPE.Heal;
		StopDistance = 2;
	}
	public override void FriendlyInit()
	{
		
	}
	private void Update()
	{
		if (isDie)
			return;

		if(healTarget != null)
		{
			if(HealingDirDistance() == true)
			{
				currentState = FriendlyState.Idle;
				if(!isHeal)
				{
					isHeal = true;
					StartCoroutine(GiveHeal());
				}
			}
			else
			{
				currentState = FriendlyState.Run;
				friendAi.SetDestination(healTarget.NavObj.position);
			}
		}
		ChangeAnimation();
	}
	private bool HealingDirDistance()
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
	private IEnumerator GiveHeal()
	{
		//healTarget.Hp += Attack;
		//healTarget.UIEnemyHealth.ValueIncrease(Attack);
		GameManager.ParticleGenerate(effectType,
			healTarget.NavObj.position);

		yield return healDelay;
	}
	


}
	