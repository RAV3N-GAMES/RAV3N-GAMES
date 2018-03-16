using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendlyChemistry : Friendly {

	private void Start()
	{
		FriendType = FRIEND_TYPE.Chemistry;
		effectType = EFFECT_TYPE.Hit;
		StopDistance = 3;
		setDelayTime = 1f;
		defaultTime = setDelayTime;
		attackDelay = new WaitForSeconds(setDelayTime);
		AttackEventMax = 10;
	}
	public override void FriendlyInit()
	{
		isDie = false;
		Hp = 130;
		AttackDamage = 25;
		AttackCount = 10;
		UiHealth.ValueInit(Hp);
		UiHealth.HealthActvie(true);
	}
	protected override void SkillEvent()
	{
		if (targetEnemy == null)
			return;

		isSkill = true;
		int temp = targetEnemy.Attack;
		targetEnemy.Attack = 0;
		GameManager.ParticleGenerate
			(EFFECT_TYPE.Invisible, targetEnemy.NavObj.position);

		StartCoroutine("SkillDelayEvent", temp);
	}
	private IEnumerator SkillDelayEvent(int temp)
	{				
		yield return new WaitForSeconds(3);
		AttackCount = 0;
		if (targetEnemy != null)
			targetEnemy.Attack = temp;
		isSkill = false;
	}
	private void OnTriggerStay(Collider other)
	{
		//적을 인식하면 적 중 
		//아군이 적을 인식하면 인식한 순으로
		//적군이 아군을 인식하면 아군 중 가장 강한 or 힐러 공격 
		if (other.CompareTag("Enemy"))
		{
			if (targetEnemy == null)
			{	
				targetEnemy = other.GetComponent<Enemy>();				
				GroupConductor.GroupRouteCall(targetEnemy);
				targetEnemy.GroupConductor.GroupRouteSet(GroupConductor.GetOrderFriendly());
			}
		}
	}
}
