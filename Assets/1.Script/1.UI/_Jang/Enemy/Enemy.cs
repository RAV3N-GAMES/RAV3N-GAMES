﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState
{
	Idle,
	Walk,
	Attack,
	Die,
    Heal,
}
public class Enemy : MonoBehaviour {
    protected const string path = "Audio/Character/";
    public AudioSource Audio;
    public AudioClip[] Clips;
    public AudioClip[] DieClip;
    public AudioClip AppearClip;
    public EnemyClusterManager ECM;
    public EnemyCluster myCluster;
    public EnemyGroup GroupConductor;       //캐릭터의 그룹을 정하고 정보를 받고 쓰기 위한 
    public HealthSystem UIEnemyHealth;      //현재 캐릭터의 체력을 나타내는 UI정보
    public Transform NavObj;                //NavMesh 상에서 움직이는 객체의 위치정보
    public Friendly targetFriend;           //타겟의 정보를 가져오기 위한
    public Transform OriginalPoint;			//기본 타겟을 저장하기위한
    public List<Friendly> NearFriendly;
    public List<Wall> NearWall;
    public Wall targetWall;
    public Trap targetTrap;
    public Enemy healTarget;
    public SecretActs targetSecret;
    public List<Trap> NearTrap;
    public DayandNight DnN;
    public bool Movable;
    public bool isHealer;
    public bool isEntered;
    public bool isLeft;
    public bool faceLeft;
    public bool isSurrounded;// 목적지(Secret / Base)까지의 경로가 막혔을 때 true
    public bool InnerSurrounded;//방 내부에서 Exitpoint까지의 경로가 막혔을 때 true
    public int priority;
    public int Group;                       //4명으로 묶인 한 그룹. 그룹의 개수에 따라 1~3의 값을 가짐.
    public int Level;
    public int Hp;                          //체력
    public int Attack;                      //공격력
    public int MaxHp;                       //최대 체력
    public float Speed;                     //이동 속도
    public float Distance;                  //거리 체크 
    public bool isDie;						//현재 죽은 상태 체크
    public bool isDefeated;                 // 거점 들어온 경우
    public bool isStolen;                 //기밀 들고 튄 경우
    public bool isSeizure;                  //탈취집단이면 true
    public int nextIdx;
    public int Exitdirection;
    public NavMeshAgent enemyAI;         //NevMeshAgent 
    protected Animator anime;               //애니메이션
    protected SphereCollider scollider;     //2D캐릭터에 붙어있는 콜라이더
    public Vector3 dest;                 //목적지 좌표
    protected int destType;                 //목적지 타입. enum ObjectType을 따름.
    protected Vector3 start;                //시작 좌표
    protected WaitForSeconds attackDelay;   //코루틴에서
    protected EnemyState currentState;      //현재 캐릭터에 상태를 나타내는 
    protected EFFECT_TYPE effectType;       //캐릭터가 사용하는 이펙트 타입
    public int PresentRoomidx;
    private bool isShoot;                   //딜레이와 공격을 맞추기위한 
    protected bool isHeal;

    protected Vector3 PrevPos;
    //기능 초기화
    public virtual void EnemyInit()
    {
        anime.Play("idle");
        scollider.enabled = true;
        enemyAI.enabled = true;
        isDie = false;
        NavObj = enemyAI.transform;
        enemyAI.speed = Speed;
        UIEnemyHealth.ValueInit(Hp);
        if (isSeizure) {
            Hp = (int)Mathf.Ceil( Hp * 0.9f);
            MaxHp = Hp;
            Attack = (int)Mathf.Ceil(Hp * 0.9f);
        }
        priority = -1;
    }

    public void HitAnimEnd()    //애니메이션에서 호출되는 함수
    {
        StartCoroutine(ShootEvent());
    }

    public void Hit()
    {
        Audio.clip = Clips[0];
        Audio.Play();
        if (targetFriend) { 
            if (targetFriend.Health(Attack))
            {
                GameManager.ParticleGenerate(effectType, targetFriend.NavObj.position);
                targetFriend.Die();
            }
        }
        else if (targetWall)
        {
            targetWall.GetDamaged(Attack);
            GameManager.ParticleGenerate(effectType, targetWall.transform.position);
            if (targetWall.IsDestroyed())
            {
                NearWall.Remove(targetWall);
                targetWall.DestoryWall();
            }
        }
    }
    public void Die()
    {
        if (isDie)
            return;
        Audio.clip = DieClip[Random.Range(0, 100) % 3];
        Audio.Play();
        StartCoroutine("DieEvent");
    }
    public bool Health(int damage)
    {
        UIEnemyHealth.ValueDecrease(damage);
        Hp -= damage;
        if (Hp > MaxHp)
            Hp = MaxHp;

        if (Hp <= 0)
            return true;
        try
        {
            GetComponentInParent<ObjectInfo>().presentHP = Hp;
        }
        catch { }

        return false;
    }
    private void KillGold()
    {
        Data_Player.addGold(ResourceManager_Player.Tbl_Player[Data_Player.Fame - 4].Reward_Kill);
    }

    public void EscapeCoroutine()
    {
        StartCoroutine(EscapeEvent());
    }

    public IEnumerator DieEvent()
    {
        isDie = true;
        isStolen = false;
        isDefeated = false;
        anime.SetTrigger("Die");
        enemyAI.enabled = false;
        scollider.enabled = false;
        UIEnemyHealth.HealthActvie(false);
        GroupConductor.RemoveEnemy(this);
        myCluster.eList.Remove(this);
        DayandNight.DeadEnemy.Add(this);
        yield return new WaitForSeconds(0.5f);
        transform.parent.gameObject.SetActive(false);
        PoolManager.current.PushEnemy(NavObj.gameObject);
    }

    public IEnumerator EscapeEvent()
    {
        isDie = false;
        isStolen = false;
        isDefeated = true;
        anime.SetTrigger("Die");
        enemyAI.enabled = false;
        scollider.enabled = false;
        UIEnemyHealth.HealthActvie(false);
        GroupConductor.RemoveEnemy(this);
        myCluster.eList.Remove(this);
        yield return new WaitForSeconds(0.5f);
        transform.parent.gameObject.SetActive(false);
        PoolManager.current.PushEnemy(NavObj.gameObject);
    }

    protected void ClearEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].GetComponentInChildren<Enemy>().EscapeCoroutine();
        }
    }

    public IEnumerator StealEvent() {
        GameObject[] Enemies = GameObject.FindGameObjectsWithTag("Enemy");
        List<Enemy> List = new List<Enemy>();
        for (int i = 0; i < Enemies.Length; i++)
        {
            Enemy e = Enemies[i].GetComponentInChildren<Enemy>();
            if (e.Group == Group && e != this)
            {
                List.Add(e);
            }
        }

        for (int i = 0; i < List.Count; i++) { 
            List[i].isDie = false;
            List[i].isStolen= true;
            List[i].isDefeated = false;
            List[i].anime.SetTrigger("Die");
            List[i].enemyAI.enabled = false;
            List[i].scollider.enabled = false;
            List[i].UIEnemyHealth.HealthActvie(false);
            List[i].GroupConductor.RemoveEnemy(this);
            List[i].myCluster.eList.Remove(this);
            yield return new WaitForSeconds(0.5f);
            List[i].transform.parent.gameObject.SetActive(false);
            PoolManager.current.PushEnemy(List[i].NavObj.gameObject);
        }
        isDie = false;
        isStolen = true;
        isDefeated = false;
        anime.SetTrigger("Die");
        enemyAI.enabled = false;
        scollider.enabled = false;
        UIEnemyHealth.HealthActvie(false);
        GroupConductor.RemoveEnemy(this);
        myCluster.eList.Remove(this);
        yield return new WaitForSeconds(0.5f);
        SecretManager.SecretList.Remove(targetSecret);
        SecretManager.SecretCount--;
        if (targetSecret)
        {
            targetSecret.GetComponentInParent<DisplayObject>().DestroyObj(true);
            Destroy(targetSecret.transform.parent.gameObject);
        }
        transform.parent.gameObject.SetActive(false);
        PoolManager.current.PushEnemy(NavObj.gameObject);
    }
    public SecretActs FindClosestSecret(Vector3 start)
    {
        SecretActs Closest = null;
        float closest = float.MaxValue;
        float tmp = 0f;

        foreach (SecretActs s in SecretManager.SecretList)
        {
            if (s)
            {
                tmp = Vector3.Distance(start, s.transform.position);
                if (s.GetComponent<SpriteRenderer>().sortingLayerName == PresentRoomidx.ToString())
                {
                    if (tmp < closest)
                    {
                        Closest = s;
                        closest = tmp;
                    }
                }
            }
        }
        return Closest;
    }
    private IEnumerator ShootEvent()
    {
        currentState = EnemyState.Idle;
        yield return attackDelay;
        isShoot = false;
    }
    
    public bool IsNear(Transform NavObjPos, Transform targetPos) {
        return (Vector3.Distance(NavObjPos.position, targetPos.position) <= enemyAI.stoppingDistance) ? true : false;
    }

    public Vector3 SetYZero(Transform t) {
        return new Vector3(t.position.x, 0, t.position.z);
    }

    public int CheckAdjacentCount()
    {
        List<int> AdjacentList = new List<int>();
        int row = PresentRoomidx / 5;
        int col = PresentRoomidx % 5;
        int nrow, ncol;
        int Min = int.MaxValue;
        int[] dx = { 0, 0, 1, -1 };
        int[] dy = { 1, -1, 0, 0 };

        for (int i = 0; i < 4; i++)
        {
            nrow = row + dx[i];
            ncol = col + dy[i];

            if (nrow < 0 || nrow > 4 || ncol < 0 || ncol > 4)
                continue;

            EnemyGroup eg = GameManager.current.enemyGroups[nrow * 5 + ncol];
            if (eg.gameObject.activeSelf)
            {
                if (Min > eg.Count)
                {
                    Min = eg.Count;
                }
            }
        }

        if (Min == int.MaxValue)//아무것도 활성화되지 않은 경우(없는 경우인거같음)
            return -1;

        for (int i = 0; i < 4; i++)
        {
            nrow = row + dx[i];
            ncol = col + dy[i];
            if (nrow < 0 || nrow > 4 || ncol < 0 || ncol > 4)
                continue;
            EnemyGroup eg = GameManager.current.enemyGroups[nrow * 5 + ncol];
            if (eg.gameObject.activeSelf)
            {
                if (Min == eg.Count)
                {
                    AdjacentList.Add(nrow * 5 + ncol);
                }
            }
        }
        int random = Random.Range(0, AdjacentList.Count);
        int result = AdjacentList[random];
        return result;
    }

    public int FindExit()
    {
        int result = -1;
        int row = PresentRoomidx / 5;
        int col = PresentRoomidx % 5;
        int nRow = nextIdx / 5;
        int nCol = nextIdx % 5;

        if (col - 1 == nCol)
        {
            result = 0;
        }
        else if (row + 1 == nRow)
        {
            result = 1;
        }
        else if (col + 1 == nCol)
        {
            result = 2;
        }
        else if (row - 1 == nRow)
        {
            result = 3;
        }
        return result;
    }
    
    protected void SetStart()
    {
        start = SetYZero(NavObj);
    }

    protected void ArrivedAction()
    {//도착 시 취하는 액션
        if (Vector3.Distance(SetYZero(NavObj), SetYZero(OriginalPoint.transform)) <= enemyAI.stoppingDistance)
        {
            ClearEnemies();//Find all Enemies and Escape them.
            DnN.changeState();
        }
        else if (isSeizure && targetSecret && Vector3.Distance(SetYZero(NavObj), SetYZero(targetSecret.transform)) <= enemyAI.stoppingDistance)
        {
            StartCoroutine(StealEvent());
        }
        else {
            if (nextIdx == -1)
                return;
            if(!(isHealer && healTarget)) { 
                Transform nextRoom = GameManager.current.enemyGroups[nextIdx].ExitPoint[Exitdirection];
                enemyAI.SetDestination(nextRoom.position);
            }
        }
    }

    protected void Flip()
    {
        faceLeft = !faceLeft;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
    protected void ChangeAnimation()
    {
        anime.SetInteger("Action", (int)currentState);
    }

    private void Awake()
    {
        Movable = true;
        Audio = GetComponent<AudioSource>();
        AppearClip= Resources.Load<AudioClip>("Audio/Character/Enemy/All") as AudioClip;

        ECM = GameObject.Find("Managers").GetComponent<EnemyClusterManager>();
        isDie = false;
        isStolen = false;
        isDefeated = false;
        PresentRoomidx = -1;
        isEntered = false;
        nextIdx = -1;
        PrevPos = Vector3.zero;
        isLeft = true;
        faceLeft = true;
        isSurrounded = false;
        InnerSurrounded = false;
        targetWall = null;
        targetTrap = null;
        healTarget = null;
        NearFriendly = new List<Friendly>();
        NearWall = new List<Wall>();
        NearTrap = new List<Trap>();
        isShoot = false;
        enemyAI = GetComponentInParent<NavMeshAgent>();
        scollider = GetComponent<SphereCollider>();
        anime = GetComponent<Animator>();
        UIEnemyHealth = GetComponentInParent<HealthSystem>();
    }

    protected void Update()
    {
        if (isDie || isStolen || isDefeated)
            return;
        SetOrder();
        EnemyActionMain();
//      GroupActionMain();
        ChangeAnimation();
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Wall"))
        {
            Wall w = col.GetComponent<Wall>();
            if (NearWall.Contains(w))
            {
                NearWall.Remove(w);
            }
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("FriendlyBody"))
        {
            Friendly e = col.GetComponentInParent<Friendly>();
            if (!NearFriendly.Contains(e))
                NearFriendly.Add(e);
        }

        if (col.CompareTag("Wall"))
        {
            Wall w = col.GetComponentInParent<Wall>();
            if (!NearWall.Contains(w))
                NearWall.Add(w);
        }
    }
    private void SetOrder()
    {
        SetOrder_Wall();
        SetOrder_Trap();
        SetOrder_Friendly();
    }

    protected void GetTrap()//해당 방에 대한 trap 얻어옴
    {
        GameObject[] Traps = GameObject.FindGameObjectsWithTag("Trap");
        Trap t;
        for (int i = 0; i < Traps.Length; i++)
        {
            SpriteRenderer s = Traps[i].GetComponent<SpriteRenderer>();
            if (!s)
                continue;
            if (s.sortingLayerName == PresentRoomidx.ToString())
            {
                t = s.GetComponent<Trap>();
                if (!NearTrap.Contains(t))
                    NearTrap.Add(t);
            }
            else
            {
                t = s.GetComponent<Trap>();
                if (NearTrap.Contains(t))
                {
                    NearTrap.Remove(t);
                }
            }
        }
        if (NearTrap[0])
        {
            targetTrap = NearTrap[0];
        }
        else
            targetTrap = null;
    }

    private void SetOrder_Trap()
    {
        for (int i = 0; i < NearTrap.Count; i++)
        {
            if (!NearTrap[i].transform.parent.gameObject.activeSelf)
            {
                NearTrap.Remove(NearTrap[i]);
                i--;
            }
        }

        if (NearTrap.Count > 1)
        {
            NearTrap.Sort(
                delegate (Trap t1, Trap t2)
                {
                    if (t1 == null)
                        return 1;
                    else if (t2 == null)
                        return -1;
                    else
                    {
                        float d1, d2;
                        d1 = Vector3.Distance(t1.transform.position, transform.position);
                        d2 = Vector3.Distance(t2.transform.position, transform.position);
                        if (d1 > d2)
                            return 1;
                        else if (d1 < d2)
                            return -1;
                        else
                            return 0;
                    }
                });
        }

        if (NearTrap.Count > 0)
            targetTrap = NearTrap[0];
        else
            targetTrap = null;
    }
    private void SetOrder_Wall()
    {
        if (NearWall.Count > 1)
        {
            NearWall.Sort(
                delegate (Wall w1, Wall w2)
                {
                    if (w1 == null)
                        return 1;
                    else if (w2 == null)
                        return -1;
                    else
                    {
                        float d1, d2;
                        d1 = Vector3.Distance(w1.transform.position, transform.position);
                        d2 = Vector3.Distance(w2.transform.position, transform.position);
                        if (d1 > d2)
                            return 1;
                        else if (d1 < d2)
                            return -1;
                        else
                            return 0;
                    }
                });
        }

        if (NearWall.Count > 0)
            targetWall = NearWall[0];
        else
            targetWall = null;
    }
    private void SetOrder_Friendly()
    {
        if (NearFriendly.Count > 1)
        {
            NearFriendly.Sort(
                delegate (Friendly f1, Friendly f2)
                {
                    if (f1 == null)
                        return 1;
                    else if (f2 == null)
                        return -1;
                    else
                    {
                        float d1, d2;
                        d1 = Vector3.Distance(f1.transform.position, transform.position);
                        d2 = Vector3.Distance(f2.transform.position, transform.position);
                        if (d1 > d2)
                            return 1;
                        else if (d1 < d2)
                            return -1;
                        else
                            return 0;
                    }
                });
        }
        if (NearFriendly.Count > 0)
        {
            targetFriend = NearFriendly[0];
        }
        else
            targetFriend = null;
    }
    
    protected void SetDestination()
    {
        if (isSeizure && targetSecret)
        {
            dest = SetYZero(targetSecret.transform);
        }
        else
        {
            dest = SetYZero(OriginalPoint.transform);
        }

        if (isHealer)
        {
            Enemy e;
            if ((e = myCluster.HurtEnemy()))
            {
                healTarget = e;
                dest = SetYZero(healTarget.NavObj);
            }
        }
    }

    protected void EnemyActionMain() {
        if (nextIdx == -1)
            return;

        SetStart();
        if (isEntered)
        {
            //해당 방의 Trap / Secret 받아오기
            NearTrap.Clear();
            GetTrap();
            targetSecret = FindClosestSecret(NavObj.transform.position);
            isEntered = false;
        }
        SetDestination();

        enemyAI.SetDestination(dest);
        enemyAI.CalculatePath(dest, enemyAI.path);

        if (enemyAI.pathStatus == NavMeshPathStatus.PathInvalid || enemyAI.pathStatus == NavMeshPathStatus.PathPartial)
        {
            if (PresentRoomidx == 0 && (!isSeizure || (isSeizure && !targetSecret)))
            {
                dest = SetYZero(OriginalPoint.transform);
            }
            else if (isSeizure && targetSecret)
            {
                dest = SetYZero(targetSecret.transform);
            }
            else
            {
                Exitdirection = FindExit();
                if (PresentRoomidx >= 0 && PresentRoomidx <= 24)
                {
                    dest = SetYZero(GameManager.current.enemyGroups[PresentRoomidx].ExitPoint[Exitdirection]);
                }
            }

            enemyAI.SetDestination(dest);
            enemyAI.CalculatePath(dest, enemyAI.path);
            if (enemyAI.pathStatus == NavMeshPathStatus.PathInvalid || enemyAI.pathStatus == NavMeshPathStatus.PathPartial)
            {
                if (targetWall || targetFriend)
                {
                    if (!isShoot)
                    {
                        enemyAI.isStopped = true;
                        isShoot = true;
                        currentState = EnemyState.Attack;
                    }
                }
                else
                {
                    currentState = EnemyState.Walk;
                    enemyAI.isStopped = true;
                    transform.parent.transform.position = Vector3.MoveTowards(enemyAI.transform.position, dest, 0.01f);
                }
            }
        }
        else {
            if (!isHealer && targetFriend)
            {
                if (!isShoot)
                {
                    enemyAI.isStopped = true;
                    isShoot = true;
                    currentState = EnemyState.Attack;
                }
            }
            else { 
                enemyAI.isStopped = false;
                currentState = EnemyState.Walk;
            }
        }
        
        /* 용병 만날 경우 전투
         * 길 막힌 경우 -> 방 내부에서 목적지 설정
         *
         * 소매치기 => 방 내부에 있는 함정 -> Exitpoint
         *  그 외 => Exitpoint
         *  
         * 1. 방 내부에서 ExitPoint까지 경로 있는 경우
         *  1.1 용병과 조우 시 공격
         *  1.2 경로 끝 도착 시(ExitPoint) 살아있는 그룹 다 모였는지 확인 후 다음 방 진출
         * 2. 방 내부에서 ExitPoint까지 경로 없는 경우
         *  2.1 용병과 조우 시 공격
         *  2.2 ExitPoint로 moveToward로 이동하며 벽 때려부숨
         *  2.3 경로 끝 도착 시(ExitPoint) 살아있는 그룹 다 모였는지 확인 후 다음 방 진출
         * 3. 방 내부에서 targetTrap까지 경로 있는 경우
         *  3.1  함정 있으면 => 해체하러감
         * 4. 방 내부에서 targetTrap까지 경로 없는 경우
         *  4.1 아군이 전투중이면 => 전투에 합류
         *  4.2 아군이 전투중 아니면 => 함정까지 movetoward로 이동하며 벽을 부숨
        */

        Distance = Vector3.Distance(start, dest);
        if (Distance <= enemyAI.stoppingDistance)
        {
            ArrivedAction();
        }

        //진행 경로에 따라 좌우 변경
        if (PrevPos == Vector3.zero)
        {
            PrevPos = transform.position;
        }
        else
        {
            if (transform.position.x - PrevPos.x > 0.01)
            {
                isLeft = false;
            }
            else
                isLeft = true;
            if (isLeft != faceLeft)
                Flip();

            PrevPos = transform.position;
        }
    }

    protected void GroupActionMain()
    {
        SetStart();
        if (isEntered)
        {
            //해당 방의 Trap / Secret 받아오기
            NearTrap.Clear();
            GetTrap();
            targetSecret = FindClosestSecret(NavObj.transform.position);
            isEntered = false;
        }
        SetDestination();

        if (isHealer && healTarget) { 
            if (IsNear(NavObj, healTarget.NavObj))
            {
                currentState = EnemyState.Idle;
                enemyAI.isStopped = true;
                if (!isHeal)
                {
                    isHeal = true;
                    StartCoroutine(GiveHeal());
                }
            }
            else
            {
                enemyAI.isStopped = false;
                currentState = EnemyState.Walk;
                enemyAI.SetDestination(dest);
            }
            goto AfterHeal;
        }
        //힐 안한 힐러 && 딜러
        if (nextIdx != -1) {
            enemyAI.CalculatePath(dest, enemyAI.path);
            if (enemyAI.pathStatus == NavMeshPathStatus.PathInvalid || enemyAI.pathStatus == NavMeshPathStatus.PathPartial)
            {//전체 경로 막힘
                // => 방 내부에서 경로 찾음
                if (!isHealer && targetFriend && IsNear(NavObj, targetFriend.transform))
                {
                    dest = SetYZero(targetFriend.transform);
                }
                else if (isSeizure && targetSecret)
                {
                    dest = SetYZero(targetSecret.transform);
                }
                else
                {
                    if (PresentRoomidx == 0)
                    {
                        dest = SetYZero(OriginalPoint.transform);
                    }
                    else
                    {
                        Exitdirection = FindExit();
                        if (PresentRoomidx >= 0 && PresentRoomidx <= 24)
                        {
                            dest = SetYZero(GameManager.current.enemyGroups[PresentRoomidx].ExitPoint[Exitdirection]);
                        }

                        if (name.Equals("MonsterPickPocket2D") && targetTrap)
                        {
                            dest = SetYZero(targetTrap.transform);
                        }
                    }
                }
                
                enemyAI.SetDestination(dest);
                if (!Movable)
                    enemyAI.isStopped = true;
                else
                    enemyAI.isStopped = false;

                enemyAI.CalculatePath(dest, enemyAI.path);
                if (enemyAI.pathStatus == NavMeshPathStatus.PathInvalid || enemyAI.pathStatus == NavMeshPathStatus.PathPartial)
                {
                    if (!isHealer && targetWall && IsNear(NavObj, targetWall.transform))
                    {
                        if (!isShoot)
                        {
                            enemyAI.isStopped = true;
                            isShoot = true;
                            currentState = EnemyState.Attack;
                        }
                    }
                    else
                    {
                        currentState = EnemyState.Walk;
                        enemyAI.isStopped = true;
                        transform.parent.transform.position = Vector3.MoveTowards(enemyAI.transform.position, dest, 0.01f);
                    }
                }
                else
                {
                    if (!isHealer && targetFriend && IsNear(NavObj, targetFriend.transform))
                    {
                        if (!isShoot)
                        {
                            enemyAI.isStopped = true;
                            isShoot = true;
                            currentState = EnemyState.Attack;
                        }
                    }
                    else
                    {
                        if (Movable)
                            enemyAI.isStopped = false;
                        currentState = EnemyState.Walk;
                    }
                }
            }
            else
            {
                if (!isHealer && targetFriend && IsNear(NavObj, targetFriend.transform))
                {
                    if (!isShoot)
                    {
                        enemyAI.isStopped = true;
                        isShoot = true;
                        currentState = EnemyState.Attack;
                    }
                }
                else if (!isHealer && targetFriend && !IsNear(NavObj, targetFriend.transform)) {
                    enemyAI.SetDestination(SetYZero(targetFriend.transform));
                    if (Movable)
                        enemyAI.isStopped = false;
                    currentState = EnemyState.Walk;
                }
                else
                {
                    if (Movable)
                        enemyAI.isStopped = false;
                    currentState = EnemyState.Walk;
                }
            }
        }
        AfterHeal:
        Distance = Vector3.Distance(start, dest);
        if (Distance <= enemyAI.stoppingDistance)
        {
            ArrivedAction();
        }

        //진행 경로에 따라 좌우 변경
        if (PrevPos == Vector3.zero)
        {
            PrevPos = transform.position;
        }
        else
        {
            if (transform.position.x - PrevPos.x > 0.01)
            {
                isLeft = false;
            }
            else
                isLeft = true;
            if (isLeft != faceLeft)
                Flip();

            PrevPos = transform.position;
        }
    }

    protected virtual IEnumerator GiveHeal() { yield return null; }
}