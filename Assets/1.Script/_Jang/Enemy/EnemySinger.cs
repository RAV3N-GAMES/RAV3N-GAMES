using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySinger : Enemy {

	WaitForSeconds healDelay = new WaitForSeconds(2f);
	Enemy healTarget = null;
	private bool isHeal = false;

	private void Start()
	{
		effectType = EFFECT_TYPE.Heal;
		StopDistance = 2;
		isDie = false;
	}
	public override void EnemyInit()
	{
		Hp = 100;
		Attack = 30;
		MaxHp = 100;
		base.EnemyInit();
	}

	private void Update()
	{
		if (isDie)
			return;
		if (GroupConductor.GetLessHpEnemy() != null)
		{
			if (healTarget != GroupConductor.GetLessHpEnemy())
			{
				healTarget = GroupConductor.GetLessHpEnemy();
			}
		}
		if (healTarget != null)
		{
			if (HealingDirDisatnce() == true)
			{
				currentState = EnemyState.Idle;
				enemyAI.enabled = false;
				if (!isHeal)
				{
					isHeal = true;
					StartCoroutine(GiveHeal());
				}
			}
			else if(HealingDirDisatnce() == false)
			{
				enemyAI.enabled = true;
				currentState = EnemyState.Walk;
				enemyAI.SetDestination(healTarget.NavObj.position);
			}
		}
		else
		{	
			OriginalDest();
		}

        if (Distance <= StopDistance)
        {
            if (isSeizure && dest != OriginalPoint.position)
            {
                StartCoroutine(StealEvent());
            }
            else
            {
                StartCoroutine(DieEvent());
            }
        }

        ChangeAnimation();
	}

    private Transform FindClosestSecret(Vector3 start)
    {
        Transform Closest = null;
        float closest = float.MaxValue;
        float tmp = 0f;
        foreach (SecretActs s in SecretManager.SecretList)
        {
            tmp = Vector3.Distance(start, s.transform.position);
            if (tmp < closest)
            {
                Closest = s.transform;
                closest = tmp;
            }
        }
        return Closest;
    }
    private void OriginalDest()
    {
        start = new Vector3(NavObj.position.x, 0, NavObj.position.z);
        if (!isSeizure)
            dest = new Vector3(OriginalPoint.position.x, 0, OriginalPoint.position.z);
        else
        {
            if (SecretManager.SecretList.Count != 0)
            {
                dest = FindClosestSecret(start).position;
            }
            else
                dest = new Vector3(OriginalPoint.position.x, 0, OriginalPoint.position.z);
        }
        Distance = Vector3.Distance(start, dest);

        if (Distance <= StopDistance)
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
            if (!isSeizure)
                enemyAI.SetDestination(OriginalPoint.position);
            else
            {
                if (SecretManager.SecretList.Count != 0)
                    enemyAI.SetDestination(FindClosestSecret(start).position);
                else
                    enemyAI.SetDestination(OriginalPoint.position);
            }
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


		if (Distance <= StopDistance)
			return true;
		else
			return false;
	}

	IEnumerator GiveHeal()
	{
		if (healTarget == null)
			yield break;
		
		healTarget.Hp += Attack;
		healTarget.UIEnemyHealth.ValueIncrease(Attack);
		GameManager.ParticleGenerate(effectType,
			healTarget.NavObj.position);
		
		yield return healDelay;
		isHeal = false;
	}


}
