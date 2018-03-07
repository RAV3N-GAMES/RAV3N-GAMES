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
	public DIRECTION DirState;
	public FRIEND_TYPE FriendType;

	public FriendlyGroup GroupConductor;
	public Enemy targetEnemy;
	public Transform NavObj;
	public Transform OriginalPoint;

	public int DamageStack;
	public int RoomIndex;
	public int Hp;
	public int MaxHp;
	public int AttackDamage;
	public float Distance;			
	public float StopDistance;          //정지할 거리 설정값
	public bool isDie;

	protected EFFECT_TYPE effectType;

	protected Animator anime;
	protected NavMeshAgent friendAi;
	protected SphereCollider collider;
	protected Transform orderTarget;

	protected Vector3 dest;
	protected Vector3 start;

	protected FriendlyState currentState;
//protected bool isDest;
	protected WaitForSeconds attackDelay;

	private bool isShoot = false;
	private void Awake()
	{
		anime = GetComponent<Animator>();
		friendAi = GetComponentInParent<NavMeshAgent>();
		collider = GetComponent<SphereCollider>();
		NavObj = friendAi.transform;		
	}
	
	public virtual void FriendlyInit() { }
	public virtual Friendly GetFriend()			 //아군 정보를 받아오기위한
	{
		return this;
	}
	public void ChangeEnemyTarget(Enemy enemy)   //타겟 변경
	{
		if (enemy == targetEnemy)
			return;

		targetEnemy = enemy;
	}
	public bool Health()						 //체력관리
	{
		if (Hp < 1)
			return true;
		++DamageStack;
		Hp -= 1;
		return false;
	}
	public void Die()							 //적이 아군을 타격했을 때 호출되는 함수
	{
		//StartCoroutine("DieEvent");
	}
	public void Hit()							 //ATTACK 애니메이션 이벤트 
	{
		if (targetEnemy == null)
			return;

		GameManager.ParticleGenerate(effectType, targetEnemy.NavObj.position);
		if (targetEnemy.Health(AttackDamage))
		{
			targetEnemy.Die();
			if (targetEnemy.GroupConductor.GroupFindEnemy() == null)
			{
				targetEnemy = null;
				GroupConductor.GroupCall(null);
				currentState = FriendlyState.Idle;
				return;
			}
			else
				targetEnemy = targetEnemy.GroupConductor.GroupFindEnemy();
		}
	}
	public void HitAnimeEnd()                    //ATTACK 애니메이션 이벤트 
	{ 
		StartCoroutine(ShootEvent());
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
			return true;
		}
		else
		{
			return false;
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

	private void Update()
	{
		if (isDie)
			return;
		FriendlyAction();
		ChangeAnimation();	
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
				friendAi.enabled = false;

				if (!isShoot)
				{
					isShoot = true;
					currentState = FriendlyState.Attack;
				}
			}
			if (DirDistance() == false)
			{
				if (currentState == FriendlyState.Attack)
					return;

				friendAi.enabled = true;
				currentState = FriendlyState.Run;
				friendAi.SetDestination(targetEnemy.NavObj.position);
			}
		}
		else
		{
			OriginalDest();
		}
	}
	
	//private IEnumerator FriendAction()
	//{
	//	while (gameObject.activeInHierarchy)
	//	{
	//		//적군의 목표거리가 일정거리(2)
	//		if (targetEnemy != null)
	//		{
	//			if (DirDistance() == true)
	//			{
	//				friendAi.enabled = false;
					
	//				yield return attackDelay;

	//				currentState = FriendlyState.Attack;
	//				GameManager.ParticleGenerate
	//					(effectType, targetEnemy.NavObj.position);

	//				//애니메이션 실행에 맞춰 파티클 생성과 적 타격이벤트 호출
	//				if (isHit == true)
	//				{
	//					if (targetEnemy.Health(AttackDamage))
	//					{
	//						targetEnemy.Die();
	//						targetEnemy = targetEnemy.GroupConductor.GroupFindEnemy();
	//					}
	//					isHit = false;
	//				}
	//				yield return null;
	//			}
	//			else if (DirDistance() == false)
	//			{
	//				friendAi.enabled = true;
	//				setState = FriendlyState.Run;
	//				friendAi.SetDestination(targetEnemy.NavObj.position);
	//				yield return null;
	//			}		
	//		}
	//		else
	//		{
	//			OriginalDest();
	//			yield return null;
	//		}
	//		yield return null;
	//	}
	//}

	private IEnumerator DieEvent()				//캐릭터가 죽었을 때 호출되는 코루틴함수
	{ 
		anime.SetInteger("Action", 0);
		anime.SetTrigger("Die");

		collider.enabled = false;
		friendAi.enabled = false;

		yield return new WaitForSeconds(1.0f);

		NavObj.gameObject.SetActive(false);
		PoolManager.current.PushFriend(NavObj.gameObject);
	}


	
	
}
