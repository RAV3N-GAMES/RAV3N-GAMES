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
        diaglogue[0] = "현상금을 받아서 전시회를 열거야";
        diaglogue[1] = "내 싸인 미리 받아 놓는게 좋을걸";
        diaglogue[2] = "외계인이 나에게 영감을 줄 것 같아..";
        diaglogue[3] = "외계인 목소리가 듣고 싶어";
    }
	public override void EnemyInit()
	{
        scollider.radius = 3.0f;
        Hp = EnemyManager.Tbl_EnemySetup[Data_Player.Fame + 106].HP;
        Attack = EnemyManager.Tbl_EnemySetup[Data_Player.Fame + 106].Attack;
        MaxHp = Hp;
        isHealer = true;
        base.EnemyInit();
        enemyAI.stoppingDistance = (float)EnemyManager.Tbl_EnemySetup[Data_Player.Fame + 106].AttackRange;
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
            yield return new WaitUntil(CalPath);
            if (isDie || isStolen || isDefeated)
                break;
            if (enemyAI.pathStatus == NavMeshPathStatus.PathInvalid || enemyAI.pathStatus == NavMeshPathStatus.PathPartial)
            {
                SetDestination2nd();
                yield return new WaitUntil(CalPath);
                if (isDie || isStolen || isDefeated)
                    break;
                if (enemyAI.pathStatus == NavMeshPathStatus.PathInvalid || enemyAI.pathStatus == NavMeshPathStatus.PathPartial)
                {
                    if (!targetFriend && !targetWall)
                    {
                        transform.parent.transform.position = Vector3.MoveTowards(enemyAI.transform.position, dest, 0.01f);
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
                    enemyAI.SetDestination(dest);
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
                enemyAI.SetDestination(dest);
                currentState = EnemyState.Walk;
                enemyAI.isStopped = false;
            }
        }
    }
}
