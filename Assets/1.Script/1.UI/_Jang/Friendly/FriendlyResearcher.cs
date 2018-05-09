using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class FriendlyResearcher : Friendly {

	Friendly healTarget = null;

	private void Start()
	{
        Clips = Resources.LoadAll<AudioClip>("Audio/Character/Friendly/Researcher") as AudioClip[];
        FriendType = FRIEND_TYPE.Resercher;
		effectType = EFFECT_TYPE.Approach;
	}
	public override void FriendlyInit()
	{
		FriendType = FRIEND_TYPE.Resercher;
		effectType = EFFECT_TYPE.Approach;
		UiHealth.ValueInit(Hp);
		UiHealth.HealthActvie(true);
	}
	protected override void Attack()
	{
		healTarget = GroupConductor.GetLessHpFriendly();
		if (healTarget == null)
			return;

        healTarget.Health(-AttackDamage);
		GameManager.ParticleGenerate(EFFECT_TYPE.Heal,healTarget.NavObj.position);
	}
	protected override void SkillEvent()
	{
		isSkill = true;
		
		StartCoroutine(FriendAttackSpeedUp());

		currentState = FriendlyState.Idle;
	}
	private IEnumerator FriendAttackSpeedUp()
	{
		Friendly friendly = GroupConductor.RandomFriendly();

		if (friendly == null)
			yield break;

		friendly.SetAnimeEvent(true);
		yield return new WaitForSeconds(4);
		friendly.SetAnimeEvent(false);
		isSkill = false;
	}

	private void OnTriggerStay(Collider other)
	{
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
	