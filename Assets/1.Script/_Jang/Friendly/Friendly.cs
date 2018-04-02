using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public enum FriendlyState
{
	Idle,
	Run,
	Attack,
}

/*
	적 타겟 순 - 힐러 -> 누적 딜량 우선 
	아군 타겟 순 - 인식 한 순부터
*/
public class Friendly : MonoBehaviour
{
	public FRIEND_TYPE FriendType;
	public FriendlyGroup GroupConductor;
	public HealthSystem UiHealth;
	public Enemy targetEnemy;
	public Transform NavObj;
	public Transform OriginalPoint;
	public bool isDie;
	public float Distance;
	public float StopDistance;          //정지할 거리 설정값
	public int DamageStack;
	public int RoomIndex;
	public int Hp;
	public int MaxHp;
	public int AttackDamage;
	public int AttackCount;
	public int AttackEventMax;

	protected EFFECT_TYPE effectType;
	protected Animator anime;
	protected NavMeshAgent friendAi;
	protected SphereCollider scollider;
	protected Vector3 dest;
	protected Vector3 start;
	protected FriendlyState currentState;
	protected WaitForSeconds attackDelay;
	protected float defaultTime;
	protected float setDelayTime;
	protected bool isShoot = false;
	protected bool isSkill = false;

	public float AISpeed
	{
		get { return friendAi.speed; }
		set { friendAi.speed = value;}
	}

	public void SetAnimeEvent(bool check)
	{
		if (check)
		{
			setDelayTime = setDelayTime * 2;
			anime.speed = 2;
		}
		else
		{
			setDelayTime = defaultTime;
			anime.speed = 1;
		}
		attackDelay = new WaitForSeconds(setDelayTime);
	}

	private void Awake()
	{
		anime = GetComponent<Animator>();
		friendAi = GetComponentInParent<NavMeshAgent>();
		scollider = GetComponent<SphereCollider>();
		UiHealth = GetComponentInParent<HealthSystem>();
		NavObj = friendAi.transform;		
	}
	private void OnEnable()
	{
		scollider.enabled = true;
		friendAi.enabled = true;
		anime.enabled = true;
	}
	public virtual void FriendlyInit()
	{
		targetEnemy = null;
		isDie = false;
		isSkill = false;
		isShoot = false;
		UiHealth.ValueInit(Hp);
		UiHealth.HealthActvie(true);
	}
	private void Update()
	{
		if (isDie)
			return;

		FriendlyAction();
		ChangeAnimation();
	}                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        
	protected virtual void Attack()
	{
		GameManager.ParticleGenerate(effectType, targetEnemy.NavObj.position);

		if (targetEnemy.Health(AttackDamage))
		{
			targetEnemy.Die();
			targetEnemy = targetEnemy.GroupConductor.GroupFindEnemy();
		}
	}
	public void Hit()							 //ATTACK 애니메이션 이벤트 
	{
		if (targetEnemy == null)
			return;


		if (AttackCount >= AttackEventMax && !isSkill)
		{
			SkillEvent();
			return;
		}
		else
		{
			++AttackCount;
			
			Attack();
			
		}
	
	}
	protected virtual void SkillEvent() { }
	public void HitAnimeEnd()                    //ATTACK 애니메이션 이벤트 
	{
		StartCoroutine("ShootEvent");
	}
	public void ChangeEnemyTarget(Enemy enemy)   //타겟 변경
	{
		if (enemy == targetEnemy)
			return;

		targetEnemy = enemy;
	}
	public void TargetEnemyChange(Enemy enemy)
	{
		if(enemy == targetEnemy)
		{			
			targetEnemy = targetEnemy.GroupConductor.GroupFindEnemy();
		}
	}
	
	public void Die()
	{
		if (isDie)
			return;

		isDie = true;
		scollider.enabled = false;
		anime.enabled = false;
		friendAi.enabled = false;
		UiHealth.HealthActvie(false);
		GroupConductor.RemoveFriendly(this);
		targetEnemy = null;
		StartCoroutine("DieEvent");
	}
	public bool Health(int damage)				
	{
		Hp -= damage;
		UiHealth.ValueDecrease(damage);
		DamageStack++;

		if (Hp <= 0)
			return true;
		
		return false;
	}
	private IEnumerator DieEvent()              
	{
		yield return new WaitForSeconds(0.5f);
		gameObject.SetActive(false);
		PoolManager.current.PushFriend(NavObj.gameObject);
	}
	private IEnumerator ShootEvent()
	{
		currentState = FriendlyState.Idle;
		yield return attackDelay;
		isShoot = false;
	}
	private void FriendlyAction()
	{
		if (targetEnemy != null)
		{
			if (DirDistance() == true)
			{
				if (!isShoot)
				{
					currentState = FriendlyState.Attack;
					isShoot = true;
				}
			}
			else if (DirDistance() == false)
			{
				if (currentState == FriendlyState.Attack)
					return;

				friendAi.enabled = true;			
				currentState = FriendlyState.Run;
				friendAi.SetDestination(targetEnemy.NavObj.position);
			}
		}
		else if(targetEnemy == null)
		{
			OriginalDest();
		}
	}

	
	protected void OriginalDest()
	{
		if (targetEnemy != null)
			return;

		dest = new Vector3(OriginalPoint.position.x, 0, OriginalPoint.position.z);
		start = new Vector3(NavObj.position.x, 0, NavObj.position.z);
		Distance = Vector3.Distance(start, dest);

		if (Distance < 1)
		{
			if (friendAi.enabled)
				friendAi.enabled = false;

			currentState = FriendlyState.Idle;
		}
		else if (Distance > 0)
		{
			if (!friendAi.enabled)
				friendAi.enabled = true;

			currentState = FriendlyState.Run;
			friendAi.SetDestination(OriginalPoint.position);
		}
	}
	protected void ChangeAnimation()
	{
		anime.SetInteger("Action", (int)currentState);
	}
	protected bool DirDistance()
	{
		if (targetEnemy == null)
			return false;

		dest = new Vector3(targetEnemy.NavObj.position.x, 0, targetEnemy.NavObj.position.z);
		start = new Vector3(NavObj.position.x, 0, NavObj.position.z);
		Distance = Vector3.Distance(dest, start);

		if (targetEnemy.NavObj.position.x > NavObj.position.x)
			transform.localScale = new Vector3(-1, 1, 1);
		else 
			transform.localScale = new Vector3(1, 1, 1);

		if (Distance <= StopDistance)
		{
			friendAi.enabled = false;
			return true;
		}
		else
		{
			friendAi.enabled = true;
			return false;
		}
	}
    private void OnTriggerEnter(Collider col) {
        if (col.CompareTag("Tile")) {
            List<Friendly> list = col.GetComponentInParent<FriendlyGroup>().friendList;

            if (!list.Find(
                delegate (Friendly f) {
                    return f == this;
                })) {
                list.Add(this);
                GroupConductor = col.GetComponentInParent<FriendlyGroup>();
            }
            
        }
    }
}
