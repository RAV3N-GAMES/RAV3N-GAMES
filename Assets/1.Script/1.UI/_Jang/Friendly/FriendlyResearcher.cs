using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendlyResearcher : Friendly {

	public Friendly healTarget = null;

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
				//targetEnemy.GroupConductor.GroupRouteSet(GroupConductor.GetOrderFriendly());
				GroupConductor.GroupRouteCall(targetEnemy);
			}
		}
	}

    protected override void FriendlyAction()
    {
        healTarget = GetHealTarget();

        if (healTarget != null)
        {
            if (DirDistance())
            {
                if (!isShoot)
                {
                    currentState = FriendlyState.Attack;
                    isShoot = true;
                }
            }
            else if (!DirDistance())
            {
                if (currentState == FriendlyState.Attack)
                    return;

                friendAi.isStopped = false;
                currentState = FriendlyState.Run;
                friendAi.SetDestination(healTarget.NavObj.position);
            }
        }
        else if (healTarget == null)
        {
            OriginalDest();
        }

        //진행 경로에 따라 좌우 변경
        if (PrevPos == Vector3.zero)
        {
            PrevPos = transform.position;
        }
        else
        {
            if (transform.position.x - PrevPos.x > 0)
            {
                isLeft = false;
            }
            else
                isLeft = true;
            if (isLeft != faceLeft)
                Flip();

            PrevPos = transform.position;
        }
    }

    protected new bool DirDistance()
    {
        if (healTarget == null)
            return false;

        dest = new Vector3(healTarget.NavObj.position.x, 0, healTarget.NavObj.position.z);
        start = new Vector3(NavObj.position.x, 0, NavObj.position.z);
        Distance = Vector3.Distance(dest, start);

        if (healTarget.NavObj.position.x > NavObj.position.x)
            transform.localScale = new Vector3(-1, 1, 1);
        else
            transform.localScale = new Vector3(1, 1, 1);

        if (Distance <= StopDistance)
        {
            friendAi.isStopped = true;
            return true;
        }
        else
        {
            friendAi.isStopped = false;
            return false;
        }
    }
}
	