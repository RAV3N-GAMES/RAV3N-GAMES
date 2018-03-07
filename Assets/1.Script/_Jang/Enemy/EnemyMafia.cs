using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//
public class EnemyMafia : Enemy {

	private void Start()
	{
		EnemyType = ENEMY_TYPE.Mafia;
		effectType = EFFECT_TYPE.Hit;
		StopDistance = 2;
		attackDelay = new WaitForSeconds(1.2f);
	}
	public override void EnemyInit()
	{
		Hp = 100;
		Attack = 15;
		base.EnemyInit();
	}
	private void OnTriggerStay(Collider other)
	{
		if (other.CompareTag("Friendly"))
		{
			if (targetFriend == null)
			{
				//우선순위 중에 해당되는 타겟이 없다면 리턴
				targetFriend = other.GetComponent<Friendly>().
					GroupConductor.GetOrderFriendly();

				GroupConductor.GroupRouteSet(targetFriend);
				//targetFriendGroup = targetFriend.GroupConductor;
			}
		}
	}
}
