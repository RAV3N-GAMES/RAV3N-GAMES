using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendlyGuard : Friendly {
	private void Start()
	{
        Clips = Resources.LoadAll<AudioClip>("Audio/Character/Friendly/Guard") as AudioClip[];
        FriendType = FRIEND_TYPE.Guard;
		effectType = EFFECT_TYPE.Approach;	
	}
	public override void FriendlyInit()
	{
		isDie = false;
		UiHealth.ValueInit(Hp);
		UiHealth.HealthActvie(true);
	}

	protected override void SkillEvent()
	{
		isSkill = true;
		GroupConductor.GroupSpeedSet(3);
		targetEnemy.GroupConductor.GroupRouteSet(this);
		StartCoroutine("AggroEvent");
	}
	//2초동안 같은 파티원 이동 속도 3배 , 타겟 우선순위 경비병으로 
	private IEnumerator AggroEvent()
	{
		yield return new WaitForSeconds(2f);
		Friendly temp = null; 
		GroupConductor.GroupSpeedSet(1);
		temp = GroupConductor.GetOrderFriendly();
		if(targetEnemy != null)
			targetEnemy.GroupConductor.GroupRouteSet(temp);
		AttackCount = 0;
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
				targetEnemy.GroupConductor.GroupRouteSet(GroupConductor.GetOrderFriendly());
				GroupConductor.GroupRouteCall(targetEnemy);
			}
		}
	}


}
