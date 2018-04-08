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

	public EnemyGroup GroupConductor;		//캐릭터의 그룹을 정하고 정보를 받고 쓰기 위한 
	public HealthSystem UIEnemyHealth;		//현재 캐릭터의 체력을 나타내는 UI정보
	public Transform NavObj;				//NavMesh 상에서 움직이는 객체의 위치정보
	public Friendly targetFriend;			//타겟의 정보를 가져오기 위한
	public Transform OriginalPoint;			//기본 타겟을 저장하기위한

    public int Level;
	public int Hp;							//체력
	public int Attack;						//공격력
	public int MaxHp;						//최대 체력
	public float Speed;						//이동 속도
	public float Distance;					//거리 체크 
	public float StopDistance;				//거리 제한 값
	public bool isDie;						//현재 죽은 상태 체크

	protected NavMeshAgent enemyAI;			//NevMeshAgent 
	protected Animator anime;				//애니메이션
	protected SphereCollider scollider;		//2D캐릭터에 붙어있는 콜라이더
	protected Vector3 dest;					//목적지 좌표
	protected Vector3 start;				//시작 좌표
	protected WaitForSeconds attackDelay;	//코루틴에서 
	protected EnemyState currentState;		//현재 캐릭터에 상태를 나타내는 
	protected EFFECT_TYPE effectType;		//캐릭터가 사용하는 이펙트 타입

	private bool isShoot;					//딜레이와 공격을 맞추기위한 
	//기능 초기화
	public virtual void EnemyInit()
	{
		anime.Play("idle");
		scollider.enabled = true;
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
		if (targetFriend == null)
			return;

		GameManager.ParticleGenerate(effectType, targetFriend.NavObj.position);

		if (targetFriend.Health(Attack))
		{
			targetFriend.Die();
            try
            {
                targetFriend = targetFriend.GroupConductor.GetOrderFriendly();
            }
            catch { }
				

		}
	}
	public void Die()
	{
		if (isDie)
			return;

		isDie = true;
		anime.SetTrigger("Die");
		enemyAI.enabled = false;
		scollider.enabled = false;
		UIEnemyHealth.HealthActvie(false);
		GroupConductor.RemoveEnemy(this);
		StartCoroutine("DieEvent");
	}
	public bool Health(int damage)
	{
		UIEnemyHealth.ValueDecrease(damage);

		if (Hp <= 10)
			return true;
		else
			Hp -= damage;

		return false;
	}
	
	private IEnumerator DieEvent()
	{
		yield return new WaitForSeconds(0.5f);
		transform.parent.gameObject.SetActive(false);
		PoolManager.current.PushEnemy(NavObj.gameObject);
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
	private IEnumerator ShootEvent()
	{
		currentState = EnemyState.Idle;
		yield return attackDelay;
		isShoot = false;
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
		else if(targetFriend == null)
		{
			OriginalDest();
		}

        if (transform.position == OriginalPoint.transform.position) {
            enemyAI.enabled = false;
            scollider.enabled = false;
            UIEnemyHealth.HealthActvie(false);
            GroupConductor.RemoveEnemy(this);
            StartCoroutine(DieEvent());
        }
    }
	protected void ChangeAnimation()
	{
		anime.SetInteger("Action", (int)currentState);
	}
	private void Awake()
	{
		enemyAI = GetComponentInParent<NavMeshAgent>();
		scollider = GetComponent<SphereCollider>();
		NavObj = enemyAI.transform;
		anime = GetComponent<Animator>();
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
