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
	아군 타겟 순 - 가장 가까운 적
*/
public class Friendly : MonoBehaviour
{
    public FRIEND_TYPE FriendType;
    public FriendlyGroup GroupConductor;
    public HealthSystem UiHealth;
    public List<Enemy> NearEnemy;
    public Enemy targetEnemy;
    public Transform NavObj;
    public Transform OriginalPoint;
    public bool isDie;
    public float Distance;
    public float StopDistance;          //정지할 거리 설정값
    public int DamageStack;
    public int RoomIndex;
    public int Level;
    public int Hp;
    public int MaxHp;
    public int AttackDamage;
    public int AttackCount;
    public int AttackEventMax;
    public int roomidx;//현재 위치한 room의 idx

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
        set { friendAi.speed = value; }
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
        AttackCount = 0;
        NavObj = friendAi.transform;
        NearEnemy = new List<Enemy>();
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

    private void Start()
    {
        //        targetEnemy = targetEnemy.GroupConductor.GroupFindEnemy();
    }
    private void Update()
    {
        if (isDie)
            return;
        SetOrder();
        FriendlyAction();
        ChangeAnimation();
    }
    protected virtual void Attack()
    {
        GameManager.ParticleGenerate(effectType, targetEnemy.NavObj.position);

        if (targetEnemy.Health(AttackDamage))
        {
            targetEnemy.Die();
            if (NearEnemy.Count > 0)
                targetEnemy = NearEnemy[1];
            else
                targetEnemy = null;
        }
    }
    public void Hit()                            //ATTACK 애니메이션 이벤트 
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
    /*public void ChangeEnemyTarget(Enemy enemy)   //타겟 변경
	{
		if (enemy == targetEnemy)
			return;

		targetEnemy = enemy;
	}*/
    public void TargetEnemyChange(Enemy enemy)
    {
        if (enemy == targetEnemy)
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
        GetComponentInParent<ObjectInfo>().presentHP = Hp;

        if (Hp <= 0) {//죽으면 true
            GetComponentInParent<ObjectInfo>().presentHP = 0;
            return true;
        }

        return false;//살면 false
    }
    private IEnumerator DieEvent()
    {
        yield return new WaitForSeconds(0.5f);
        transform.parent.gameObject.SetActive(false);
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

                friendAi.enabled = true;
                currentState = FriendlyState.Run;
                friendAi.SetDestination(targetEnemy.NavObj.position);
            }
        }
        else if (targetEnemy == null)
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
                }))
            {
                list.Add(this);
                GroupConductor = col.GetComponentInParent<FriendlyGroup>();
            }
            roomidx = int.Parse(col.transform.gameObject.GetComponentInParent<TileManager>().name);
        }
        if (col.CompareTag("Enemy")) {
            Enemy e = col.GetComponent<Enemy>();
            NearEnemy.Add(e);
        }
    }

/*    private void OnTriggerStay(Collider col) {
        if (col.CompareTag("Enemy")) {
            if (!col.gameObject.activeSelf) {
                NearEnemy.Remove(col.GetComponent<Enemy>());
            }
        }
    }

    private void OnTriggerExit(Collider col) {
        if (col.CompareTag("Enemy")) {
            NearEnemy.Remove(col.GetComponent<Enemy>());
            Debug.Log("Enemy: " + col.name+" Escpaed");
        }
    }
    */
    private void SetOrder() {
        for (int i = 0; i < NearEnemy.Count; i++) {
            if (!NearEnemy[i].transform.parent.gameObject.activeSelf)
            {
                NearEnemy.Remove(NearEnemy[i]);
                i--;
            }
        }
        if (NearEnemy.Count > 1) {
        NearEnemy.Sort(
            delegate (Enemy e1, Enemy e2)
            {
                if (e1 == null)
                    return 1;
                else if (e2 == null)
                    return -1;
                else {
                    float d1, d2;
                    d1 = Vector3.Distance(e1.transform.position, transform.position);
                    d2 = Vector3.Distance(e2.transform.position, transform.position);
                    if (d1 > d2)
                        return 1;
                    else if (d1 < d2)
                        return -1;
                    else
                        return 0;
                }
            });
        }
        if (NearEnemy.Count > 0)
        {
            targetEnemy = NearEnemy[0];
        }
        else
            targetEnemy = null;
    }
}
