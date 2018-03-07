using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendlyGuard : Friendly {
	private void Start()
	{		
		FriendType = FRIEND_TYPE.Guard;
		effectType = EFFECT_TYPE.Approach;
		Hp = 200;
		AttackDamage = 15;
		StopDistance = 0.5f;
		attackDelay = new WaitForSeconds(1f);
	}
	//현재 캐릭터의 정보를 가져오기 위한
	public override Friendly GetFriend()
	{
		return this;
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
				GroupConductor.GroupCall(targetEnemy);
			}
		}
	}


}
