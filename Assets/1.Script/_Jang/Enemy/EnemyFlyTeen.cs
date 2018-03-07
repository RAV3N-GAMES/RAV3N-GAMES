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
