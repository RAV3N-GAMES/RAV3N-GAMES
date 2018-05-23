using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySinger : Enemy {
    WaitForSeconds healDelay = new WaitForSeconds(2f);

	private void Start()
	{
        Clips = Resources.LoadAll<AudioClip>(path + "Enemy/Singer") as AudioClip[];
        DieClip = Resources.LoadAll<AudioClip>(path + "Die/Woman") as AudioClip[];
        effectType = EFFECT_TYPE.Heal;
		isDie = false;
	}
	public override void EnemyInit()
	{
        scollider.radius = (float)EnemyManager.Tbl_EnemySetup[Data_Player.Fame + 106].AttackRange * 2;
        Hp = EnemyManager.Tbl_EnemySetup[Data_Player.Fame + 106].HP;
        Attack = EnemyManager.Tbl_EnemySetup[Data_Player.Fame + 106].Attack;
        MaxHp = Hp;
        isHealer = true;
        base.EnemyInit();
        enemyAI.stoppingDistance = scollider.radius;
    }

    protected override IEnumerator GiveHeal()
    {
        if (healTarget == null)
            yield break;

        healTarget.Health(-Attack);
        GameManager.ParticleGenerate(effectType,
            healTarget.NavObj.position);

        yield return healDelay;
        isHeal = false;
    }

    protected override IEnumerator EnemyAction()
    {
        while (!(isDie || isStolen || isDefeated))
        {
            SetStart();
            SetDestination();
            enemyAI.SetDestination(dest);
            yield return new WaitUntil(CalPath);
            if (isDie || isStolen || isDefeated)
                break;
            if (enemyAI.pathStatus == NavMeshPathStatus.PathInvalid || enemyAI.pathStatus == NavMeshPathStatus.PathPartial)
            {
                SetDestination2nd();
                enemyAI.SetDestination(dest);
                yield return new WaitUntil(CalPath);
                if (isDie || isStolen || isDefeated)
                    break;
                if (enemyAI.pathStatus == NavMeshPathStatus.PathInvalid || enemyAI.pathStatus == NavMeshPathStatus.PathPartial)
                {
                    if (!targetFriend && !targetWall)
                    {
                        transform.parent.transform.position = Vector3.MoveTowards(enemyAI.transform.position, dest, 0.02f);
                        currentState = EnemyState.Walk;
                        enemyAI.isStopped = false;
                    }
                    else
                    {
                        currentState = EnemyState.Idle;
                        enemyAI.isStopped = true;
                    }
                }
                else if (targetFriend && IsNear(NavObj, targetFriend.transform))
                {
                    currentState = EnemyState.Idle;
                    enemyAI.isStopped = true;

                }
                else
                {
                    currentState = EnemyState.Walk;
                    enemyAI.isStopped = false;
                }
            }
            else if (healTarget && IsNear(NavObj, healTarget.transform)) {
                if (!isHeal) {
                    currentState = EnemyState.Heal;
                    enemyAI.isStopped = true;
                    isHeal = true;
                    StartCoroutine(GiveHeal());
                }
            }
            else if (targetFriend && IsNear(NavObj, targetFriend.transform))
            {
                currentState = EnemyState.Idle;
                enemyAI.isStopped = true;
            }
            else
            {
                currentState = EnemyState.Walk;
                enemyAI.isStopped = false;
            }
        }
    }
}
