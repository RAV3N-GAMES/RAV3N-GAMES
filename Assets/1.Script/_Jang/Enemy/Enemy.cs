using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState
{
	Idle,
	Walk,
	Attack,
	Die,
}

public class Enemy : MonoBehaviour {

	public EnemyGroup GroupConductor;
	public HealthSystem UIEnemyHealth;
	public Transform NavObj;
	public Transform Target;
	public Friendly targetFriend;
	public Transform OriginalPoint;

	public int Hp;				//체력
	public int Attack;			//공격력
	public int MaxHp;			//최대 체력
	public float Speed;			
	public float Distance;		//거리 체크 
	public float StopDistance;	//거리 제한 값
	public bool isHit;			
	public bool isDie;
	public ENEMY_TYPE EnemyType;

	protected NavMeshAgent enemyAI;
	protected Animator anime;
	protected Rigidbody rigidy;
	protected SphereCollider collider;
	protected Vector3 dest;
	protected Vector3 start;
	protected WaitForSeconds attackDelay;
	protected EnemyState currentState;
	protected EFFECT_TYPE effectType;

	private bool isShoot;
	//기능 초기화
	public virtual void EnemyInit()
	{
		anime.Play("idle");
		collider.enabled = true;
		enemyAI.enabled = true;
		isDie = false;
		enemyAI.speed = Speed;
		UIEnemyHealth.ValueInit(Hp);
		MaxHp = Hp;
	}
	public void HitAnimEnd()    //애니메이션에서 호출되는 함수
	{
		StartCoroutine(ShootEvent());
	}
	public void Hit()
	{
        //Wall 타격도 생각해야됨



		if (targetFriend == null)
			return;

		GameManager.ParticleGenerate(effectType, targetFriend.NavObj.position);
		// ---- 아군 타격 ----
	}
	public void Die()
	{
		if (isDie)
			return;

		isDie = true;
		anime.SetTrigger("Die");
		enemyAI.enabled = false;
		collider.enabled = false;
		UIEnemyHealth.HealthActvie(false);
		GroupConductor.RemoveEnemy(this);
		StartCoroutine(DieEvent());
	}
	public bool Health(int damage)
	{
        Debug.Log("Health Called in " + this);
		UIEnemyHealth.ValueDecrease(damage);

		if (Hp <= 10)
			return true;
		else
			Hp -= damage;

		return false;
	}
	protected void ChangeAnimation()
	{
		anime.SetInteger("Action", (int)currentState);
	}
	private void EnemyAction()
	{
		if (targetFriend != null)
		{
			if (DirDistance() == true)
			{
				enemyAI.enabled = false;

				if (!isShoot)
				{
					isShoot = true;
					currentState = EnemyState.Attack;
				}
			}
			if (DirDistance() == false)
			{
				if (currentState == EnemyState.Attack)
					return;

				enemyAI.enabled = true;
				currentState = EnemyState.Walk;
				enemyAI.SetDestination(targetFriend.NavObj.position);
			}
		}
		else
		{
			OriginalDest();
		}
	}
	private bool DirDistance()
	{
		if (targetFriend == null)
			return false;

		dest = new Vector3(targetFriend.NavObj.position.x, 0, targetFriend.NavObj.position.z);
		start = new Vector3(NavObj.position.x, 0, NavObj.position.z);
		Distance = Vector3.Distance(dest, start);

		if (targetFriend.NavObj.position.x > NavObj.position.x)
			transform.localScale = new Vector3(-1, 1, 1);
		else
			transform.localScale = new Vector3(1, 1, 1);
		if (Distance < StopDistance)
		{
			return true;
		}
		else
		{
			return false;
		}
	}
	private void OriginalDest()
	{
		if (targetFriend != null)
			return;

		Target = OriginalPoint;
		dest = new Vector3(OriginalPoint.position.x, 0, OriginalPoint.position.z);
		start = new Vector3(NavObj.position.x, 0, NavObj.position.z);
		Distance = Vector3.Distance(start, dest);

		if (Distance < StopDistance)
		{
			if (enemyAI.enabled)
			{
				enemyAI.enabled = false;
			}
			currentState = EnemyState.Idle;
		}
		else if (Distance > StopDistance)
		{
			if (currentState == EnemyState.Attack)
				return;

			if (!enemyAI.enabled)
			{
				enemyAI.enabled = true;
			}
			currentState = EnemyState.Walk;
			enemyAI.SetDestination(OriginalPoint.position);
		}

	}
	private IEnumerator DieEvent()
	{		
		yield return new WaitForSeconds(1);		
		PoolManager.current.PushEnemy(NavObj.gameObject);
	}
	private IEnumerator ShootEvent()
	{
		currentState = EnemyState.Idle;
		yield return attackDelay;
		isShoot = false;
	}
	private void Awake()
	{
		enemyAI = GetComponentInParent<NavMeshAgent>();
		collider = GetComponent<SphereCollider>();
		NavObj = enemyAI.transform;
		anime = GetComponent<Animator>();
		rigidy = GetComponent<Rigidbody>();
		UIEnemyHealth = GetComponentInParent<HealthSystem>();
	}
	private void Update()
	{
		if (isDie)
			return;

		EnemyAction();
		ChangeAnimation();
	}
}
