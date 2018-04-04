using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//2타일 거리 전방 60도 범위
//직경 5타일 범위에 공격력의 150% 데미지(소수점 아래 올림)
public class FriendlyArmy : Friendly {

	private void Start()
	{
		FriendType = FRIEND_TYPE.Army;
		effectType = EFFECT_TYPE.Hit;
		StopDistance = 2;
		setDelayTime = 1.2f;
		defaultTime = setDelayTime;
		attackDelay = new WaitForSeconds(setDelayTime);		
		AttackEventMax = 8;
		AttackCount = AttackEventMax;
	}
	public override void FriendlyInit()
	{
		AttackEventMax = 8;
		AttackCount = AttackEventMax;
		Hp = 120;
		AttackDamage = 22;
		base.FriendlyInit();
	}
	protected override void SkillEvent()
	{
		isSkill = true;
		int perDamage =  Mathf.RoundToInt(AttackDamage * 1.5f);
		GameManager.ParticleGenerate(EFFECT_TYPE.Bang, targetEnemy.NavObj.position);
		targetEnemy.GroupConductor.RangeInEnemy(targetEnemy.NavObj.position, perDamage,5.0f ,EFFECT_TYPE.Bang);
		isSkill = false;
	}

	// 전방 60도 안에 있는지 체크 > 벡터 내적으로 
	private bool InnerSee(Transform targetNav)
	{
		return false;
	}

	protected override void Attack()
	{	
		targetEnemy.GroupConductor.RangeInEnemy(targetEnemy.NavObj.position, 
			AttackDamage, 3.0f, effectType);
	}
	private void OnTriggerStay(Collider other)
	{
		if(other.CompareTag("Enemy"))
		{
			if (targetEnemy == null)
			{
				targetEnemy = other.GetComponent<Enemy>();
				targetEnemy.GroupConductor.GroupRouteSet(GroupConductor.GetOrderFriendly());
				GroupConductor.GroupRouteCall(targetEnemy);
			}
		}
	}

}
