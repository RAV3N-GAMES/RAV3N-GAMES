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

    public int Group;                       //4명으로 묶인 한 그룹
    public int Level;
	public int Hp;							//체력
	public int Attack;						//공격력
	public int MaxHp;						//최대 체력
	public float Speed;						//이동 속도
	public float Distance;					//거리 체크 
	public float StopDistance;				//거리 제한 값
	public bool isDie;						//현재 죽은 상태 체크
    public bool isSeizure;                  //탈취집단이면 true
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

		if (targetFriend.Health(Attack))
		{
            GameManager.ParticleGenerate(effectType, targetFriend.NavObj.position);

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
        StartCoroutine("DieEvent");
	}
    public bool Health(int damage)
    {
        UIEnemyHealth.ValueDecrease(damage);
        if (Hp <= 10) {
            Hp = 0;
            return true;
        }
        else
            Hp -= damage;
        try
        {
            GetComponentInParent<ObjectInfo>().presentHP = Hp;
        }
        catch { }
        
        return false;
    }
    private void KillGold() {
        Data_Player.addGold(ResourceManager_Player.Tbl_Player[Data_Player.Fame - 4].Reward_Kill);
    }

	protected IEnumerator DieEvent()
	{
        isDie = true;
        anime.SetTrigger("Die");
        enemyAI.enabled = false;
        scollider.enabled = false;
        UIEnemyHealth.HealthActvie(false);
        GroupConductor.RemoveEnemy(this);
        
        yield return new WaitForSeconds(0.5f);
        KillGold();
		transform.parent.gameObject.SetActive(false);
		PoolManager.current.PushEnemy(NavObj.gameObject);
	}

    protected IEnumerator StealEvent() {
        isDie = true;
        anime.SetTrigger("Die");
        enemyAI.enabled = false;
        scollider.enabled = false;
        UIEnemyHealth.HealthActvie(false);
        GroupConductor.RemoveEnemy(this);
        yield return new WaitForSeconds(0.5f);
        transform.parent.gameObject.SetActive(false);
        PoolManager.current.PushEnemy(NavObj.gameObject);
    }
	private bool DirDistance()
	{//targetFriend한테 가는지 안가는지 결정.
        //targetFriend까지의 거리 Distance도 계산
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
        //true: 때릴 친구랑 붙어있음
        //false: 때릴 친구가 없거나 붙어있지 않음.
	}

    private Transform FindClosestSecret(Vector3 start) {
        Transform Closest=null;
        float closest = float.MaxValue;
        float tmp = 0f;
        foreach (SecretActs s in SecretManager.SecretList) {
            tmp = Vector3.Distance(start, s.transform.position);
            if (tmp < closest) {
                Closest = s.transform;
                closest = tmp;
            }
        }
        return Closest;
    }
    private void OriginalDest()
    {
        if (targetFriend != null)
            return;
        Transform d=null;
        start = new Vector3(NavObj.position.x, 0, NavObj.position.z);
        if (!isSeizure) {
            dest = new Vector3(OriginalPoint.position.x, 0, OriginalPoint.position.z);
        }
        else {
            if (SecretManager.SecretList.Count != 0) {
                d = FindClosestSecret(start);
                dest = d.position;
            }
            else
                dest = new Vector3(OriginalPoint.position.x, 0, OriginalPoint.position.z);
        }
        
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
            if(!isSeizure)
    			enemyAI.SetDestination(OriginalPoint.position);
            else {
                if (SecretManager.SecretList.Count != 0)
                    enemyAI.SetDestination(d.position);
                else
                    enemyAI.SetDestination(OriginalPoint.position);
            }
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
			if (DirDistance())
			{
				enemyAI.enabled = false;

				if (!isShoot)
				{
					isShoot = true;
					currentState = EnemyState.Attack;
				}
			}
			if (!DirDistance())
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

        if (Distance <= StopDistance) {//목적지에 도달할 경우(목적지: base / Secret 중 1)
            if (isSeizure && dest != OriginalPoint.position) {//Secret 도착
                StartCoroutine(StealEvent());
            }
            else { //base 도착
                //DieEvent를 호출하여 죽이지만 DeadEnemy에는 안들어감.
                StartCoroutine(DieEvent());
            }
            //targetFriend 도착은 이미 위에서 처리
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
