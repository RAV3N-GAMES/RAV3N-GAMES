using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPickPocket : Enemy {
    public Trap targetTrap;
    public bool IsDisassemble;

    // Use this for initialization
    private void Start()
    {
        Clips = Resources.LoadAll<AudioClip>(path + "Enemy/PickPocket") as AudioClip[];
        DieClip = Resources.LoadAll<AudioClip>(path + "Die/Man") as AudioClip[];
        effectType = EFFECT_TYPE.Approach;
        attackDelay = new WaitForSeconds(1f);
        targetTrap = null;
        IsDisassemble = false;
        diaglogue[0] = "함정이있다고? 분해하는게 내 전문이야";
        diaglogue[1] = "현상금의 지분은 저도 꽤 있습니다";
        diaglogue[2] = "눈 깜빡하면 내가 훔쳐간다구";
        diaglogue[3] = "내 손은 눈보다 빠르지";
    }
    public override void EnemyInit()
    {
        scollider.radius = 3.0f;
        Hp = EnemyManager.Tbl_EnemySetup[Data_Player.Fame + 40].HP;
        Attack = EnemyManager.Tbl_EnemySetup[Data_Player.Fame + 40].Attack;
        MaxHp = Hp;
        isHealer = false;
        base.EnemyInit();
        enemyAI.stoppingDistance = (float)EnemyManager.Tbl_EnemySetup[Data_Player.Fame + 40].AttackRange;
        enemyAI.speed = 1.0f;
        Stoppingdistance = enemyAI.stoppingDistance;
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
                if (!targetTrap) {
                    SetDestination2nd();
                }
                yield return new WaitUntil(CalPath);
                if (isDie || isStolen || isDefeated)
                    break;
                if (enemyAI.pathStatus == NavMeshPathStatus.PathInvalid || enemyAI.pathStatus == NavMeshPathStatus.PathPartial)
                {
                    if (targetTrap && !targetFriend)
                    {
                        if (!IsNear(NavObj, targetTrap.transform))
                        {
                            transform.parent.transform.position = Vector3.MoveTowards(enemyAI.transform.position, dest, 0.01f);
                            currentState = EnemyState.Walk;
                            enemyAI.isStopped = false;
                        }
                    }
                    else
                    {
                        if ((targetFriend && IsNear(NavObj, targetFriend.NavObj)) || (targetWall && IsNear(NavObj, targetWall.transform)))
                        {
                            if (!isShoot)
                            {
                                currentState = EnemyState.Attack;
                                isShoot = true;
                                enemyAI.isStopped = true;
                            }
                        }
                        else {
                            transform.parent.transform.position = Vector3.MoveTowards(enemyAI.transform.position, dest, 0.01f);
                            currentState = EnemyState.Walk;
                            enemyAI.isStopped = false;
                        }
                    }
                }
                else if (targetFriend && IsNear(NavObj, targetFriend.transform))
                {
                    if (!isShoot)
                    {
                        currentState = EnemyState.Attack;
                        isShoot = true;
                        enemyAI.isStopped = true;
                    }
                }
                else
                {
                    enemyAI.SetDestination(dest);
                    currentState = EnemyState.Walk;
                    enemyAI.isStopped = false;
                }
            }
            else if (targetFriend && IsNear(NavObj, targetFriend.transform))
            {
                if (!isShoot)
                {
                    currentState = EnemyState.Attack;
                    isShoot = true;
                    enemyAI.isStopped = true;
                }
            }
            else
            {
                enemyAI.SetDestination(dest);
                currentState = EnemyState.Walk;
                enemyAI.isStopped = false;
            }
        }
    }

    public IEnumerator DisassemblyTrap() {
        IsDisassemble = true;
        yield return new WaitForSeconds(5.0f);
        Trap tmp = targetTrap;
        if(targetTrap && NearTrap.Contains(targetTrap)) { 
            NearTrap.Remove(targetTrap);
        }
        if (NearTrap.Count > 0) { 
            targetTrap = NearTrap[0];
        }
        else { 
            targetTrap = null;
        }
        if (tmp) {
            tmp.displayobject.DestroyObj(true);
            Destroy(tmp.transform.parent.gameObject);
        } 
        IsDisassemble = false;
    }

    
    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("FriendlyBody"))
        {
            Friendly e = col.GetComponentInParent<Friendly>();
            if (!myCluster.GroupNearFriend.Contains(e))
            {
                myCluster.GroupNearFriend.Add(e);
                myCluster.GetPriorFriend();
            }
        }

        if (col.CompareTag("Wall"))
        {
            Wall w = col.GetComponentInParent<Wall>();
            if (!myCluster.GroupNearWall.Contains(w)) { 
                myCluster.GroupNearWall.Add(w);
                myCluster.SetOrderWall();
            }

            if (!NearWall.Contains(w))
            {
                NearWall.Add(w);
                targetWall = w;
            }
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Wall"))
        {
            Wall w = col.GetComponent<Wall>();
            if (myCluster.GroupNearWall.Contains(w))
            {
                myCluster.GroupNearWall.Remove(w);
                myCluster.SetOrderWall();
            }

            if (NearWall.Contains(w)) {
                NearWall.Remove(w);
            }

            if (NearWall.Count > 0)
                targetWall = NearWall[0];
        }

        if (col.CompareTag("FriendlyBody"))
        {
            Friendly f = col.GetComponentInParent<Friendly>();

            if (myCluster.GroupNearFriend.Contains(f))
            {
                myCluster.GroupNearFriend.Remove(f);
                myCluster.GetPriorFriend();
            }
        }
    }
}
