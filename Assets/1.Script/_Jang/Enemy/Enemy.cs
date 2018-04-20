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
}
public class Enemy : MonoBehaviour {
    public EnemyGroup GroupConductor;       //캐릭터의 그룹을 정하고 정보를 받고 쓰기 위한 
    public HealthSystem UIEnemyHealth;      //현재 캐릭터의 체력을 나타내는 UI정보
    public Transform NavObj;                //NavMesh 상에서 움직이는 객체의 위치정보
    public Friendly targetFriend;           //타겟의 정보를 가져오기 위한
    public Transform OriginalPoint;			//기본 타겟을 저장하기위한
    public List<Friendly> NearFriendly;
    public List<Wall> NearWall;
    public Wall targetWall;
    public Trap targetTrap;
    public SecretActs targetSecret;
    public List<Trap> NearTrap;

    public bool isHealer;
    public bool isSurrounded;// 목적지(Secret / Base)까지의 경로가 막혔을 때 true
    public int priority;
    public int Group;                       //4명으로 묶인 한 그룹
    public int Level;
    public int Hp;                          //체력
    public int Attack;                      //공격력
    public int MaxHp;                       //최대 체력
    public float Speed;                     //이동 속도
    public float Distance;                  //거리 체크 
    public float StopDistance;              //거리 제한 값
    public bool isDie;						//현재 죽은 상태 체크
    public bool isSeizure;                  //탈취집단이면 true
    protected NavMeshAgent enemyAI;         //NevMeshAgent 
    protected Animator anime;               //애니메이션
    protected SphereCollider scollider;     //2D캐릭터에 붙어있는 콜라이더
    public Vector3 dest;                 //목적지 좌표
    protected int destType;                 //목적지 타입. enum ObjectType을 따름.
    protected Vector3 start;                //시작 좌표
    protected WaitForSeconds attackDelay;   //코루틴에서
    protected EnemyState currentState;      //현재 캐릭터에 상태를 나타내는 
    protected EFFECT_TYPE effectType;       //캐릭터가 사용하는 이펙트 타입

    private bool isShoot;                   //딜레이와 공격을 맞추기위한 
                                            //기능 초기화
    public virtual void EnemyInit()
    {
        anime.Play("idle");
        scollider.enabled = true;
        enemyAI.enabled = true;
        isDie = false;
        NavObj=enemyAI.transform;
        enemyAI.speed = Speed;
        UIEnemyHealth.ValueInit(Hp);
        MaxHp = Hp;
        priority = -1;
    }

    public void HitAnimEnd()    //애니메이션에서 호출되는 함수
    {
        StartCoroutine(ShootEvent());
    }

    public void Hit()
    {
        if (targetWall) {
            targetWall.GetDamaged(Attack);
            if (targetWall.IsDestroyed()) {
                NearWall.Remove(targetWall);
                targetWall.DestoryWall();
            }
        }
        
        if (targetFriend == null)
            return;

        if (targetFriend.Health(Attack))
        {
            GameManager.ParticleGenerate(effectType, targetFriend.NavObj.position);
                
            targetFriend.Die();
            /*try
            {
                targetFriend = targetFriend.GroupConductor.GetOrderFriendly();
            }
            catch { }*/
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
        Debug.Log("Die Event: "+ name);
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

    protected IEnumerator EscapeEvent()
    {
        Debug.Log("Escape Event: "+name);
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

    protected IEnumerator StealEvent() {
        Debug.Log(name+": StealEvent Triggered");
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

    private bool DirDistance(GameObject g, int Type)
    {//해당 게임 오브젝트한테 가는지 안가는지 결정.
     //게임 오브젝트까지의 거리 Distance도 계산
        if (g == null)
            return false;

        start = new Vector3(NavObj.position.x, 0, NavObj.position.z);
        if (Type == (int)ObjectType.Friendly)
            dest = new Vector3(targetFriend.NavObj.position.x, 0, targetFriend.NavObj.position.z);
        else if (Type == (int)ObjectType.Building)
            dest = new Vector3(targetWall.transform.position.x, 0, targetWall.transform.position.z);
        else if (Type == (int)ObjectType.Trap)
            dest = new Vector3(targetTrap.transform.position.x, 0, targetTrap.transform.position.z);
        else if (Type == (int)ObjectType.Secret)
            dest = new Vector3(targetSecret.transform.position.x, 0, targetTrap.transform.position.z);
        
        Distance = Vector3.Distance(dest, start);

        if (Type == (int)ObjectType.Friendly && targetFriend != null) { // 없어도??
            if (targetFriend.NavObj.position.x > NavObj.position.x)
                transform.localScale = new Vector3(-1, 0, 1);
            else
                transform.localScale = new Vector3(1, 0, 1);
        }
        //Debug.Log(name + " - Distance: " + Distance + " StopDistance: " + StopDistance+ " DirDistance: "+ ((Distance<StopDistance)? true : false).ToString());

        if (Type == (int)ObjectType.Building)
        {
            if (Distance < StopDistance * 3)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else {
            if (Distance < StopDistance * 3)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //true: 때릴 게임 오브젝트랑 붙어있음
        //false: 때릴 오브젝트가 없거나 붙어있지 않음.
    }

    private SecretActs FindClosestSecret(Vector3 start) {
        SecretActs Closest = null;
        float closest = float.MaxValue;
        float tmp = 0f;
        foreach (SecretActs s in SecretManager.SecretList) {
            tmp = Vector3.Distance(start, s.transform.position);
            if (tmp < closest) {
                Closest = s;
                closest = tmp;
            }
        }
        return Closest;
    }
    private void OriginalDest()//targetFriend, targetSecret, targetWall, targetTrap에 따라 dest를 설정
    {
        //targetTrap -> PickPocket 한정
        //그 외 => Surrounded -> target wall 1순위
        //else { targetFriend => 2 순위
        //targetSecret / OriginalDestination
        start = new Vector3(NavObj.position.x, 0, NavObj.position.z);

        if (name == "PickPocket")
        {
            if(!targetTrap)
                dest = new Vector3(targetTrap.transform.position.x, 0, targetTrap.transform.position.z);
            else
                dest= new Vector3(OriginalPoint.position.x, 0, OriginalPoint.position.z);
        }
        else {
            if (isSurrounded)
            {//Destination==targetWall
                if (!targetWall)
                    dest = new Vector3(targetWall.transform.position.x, 0, targetWall.transform.position.z);
                else
                    dest = new Vector3(OriginalPoint.position.x, 0, OriginalPoint.position.z);
            }
            else {
                if (targetFriend != null)
                    return;
                if (!isSeizure)
                {
                    dest = new Vector3(OriginalPoint.position.x, 0, OriginalPoint.position.z);
                }
                else
                {
                    if (SecretManager.SecretList.Count != 0)
                    {
                        targetSecret = FindClosestSecret(start);
                        dest = targetSecret.transform.position;
                    }
                    else
                        dest = new Vector3(OriginalPoint.position.x, 0, OriginalPoint.position.z);
                }
            }
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
            if(!isSurrounded)
                enemyAI.SetDestination(dest);
        }
    }
    
    private IEnumerator ShootEvent()
    {
        currentState = EnemyState.Idle;
        yield return attackDelay;
        isShoot = false;
    }
    private void SetStart() {
        start = new Vector3(NavObj.position.x, 0, NavObj.position.z);
    }

    private bool IsNear(Transform NavObjPos, Transform targetPos) {
        return (Vector3.Distance(NavObjPos.position, targetPos.position) <= StopDistance) ? true : false;
    }

    private Vector3 SetYZero(Transform t) {
        return new Vector3(t.position.x, 0, t.position.z);
    }
    protected void SetDest(Enemy e)
    {
        /*
         * 우선순위
         * 소매치기 && 함정 존재 => 함정 (Priority: 0)
         * Surrounded => 주변 벽(Priority: 1 - 힐러 포함)
         * 공격수)
         *  1. 주변 용병 (Priority : 2)
         *      용병 내 우선순위는 소팅으로 정렬
         *  2.1 탈취단 && 기밀 존재 => 기밀(Priority: 3)
         *  2.2 그 외 => 베이스(Priority: 4)
         *  
         *  힐러)
         *   1. 주변 다친 아군(Priority: 2)
         *   2. 탈취단 && 기밀 존재 => 기밀(Priority: 3)
         *   3. 그 외 => 베이스(Priority: 4)
         */
        if (e == null)
            return;

        if (e.name == "PickPocket" && targetTrap) {
            dest = SetYZero(targetTrap.transform);
            priority = 0;
            return;
        }

        if (!isHealer) {
            if (isSurrounded && targetWall && IsNear(NavObj.transform, targetWall.transform)) {
                dest = SetYZero(targetWall.transform);
                priority = 1;
            }
            else if (targetFriend && IsNear(NavObj.transform, targetFriend.transform))
            {//아군 용병이 공격범위 안에 있을 경우 dest는 용병
                dest = SetYZero(targetFriend.transform);
                priority = 2;
            }
            else {
                if (isSeizure && targetSecret)
                {
                    dest = SetYZero(targetSecret.transform);
                    priority = 3;
                }
                else {
                    dest = SetYZero(OriginalPoint.transform);
                    priority = 4;
                }
            }
        }
    }

    private void SetAIDestination() {
        switch (priority) {
            case 0:
                enemyAI.SetDestination(SetYZero(targetTrap.transform));
                break;
            case 1:
                enemyAI.SetDestination(SetYZero(OriginalPoint.transform));
                break;
            case 2:
                if (!isHealer)
                {
                    enemyAI.SetDestination(SetYZero(targetFriend.transform));
                }
                else {//다친 Enemy 한테로 destination 설정

                }
                break;
            case 3:
                enemyAI.SetDestination(SetYZero(targetSecret.transform));
                break;
            case 4:
                enemyAI.SetDestination(SetYZero(OriginalPoint.transform));
                break;
        }
    }

    private void IsArrived() {//도착 시 취하는 액션
        switch (priority) {
            case 3:
                StartCoroutine(StealEvent());
                break;
            case 4:
                StartCoroutine(EscapeEvent());
                break;
        }
    }

    private void EnemyAction()
    {
        //시작지 설정
        //목적지 설정(우선순위별)
        //행동 설정

        SetStart();
        SetDest(this);
        
        switch (priority) {
            case 0:
                //소매치기가 함정 타게팅 했을 때
                break;
            case 1://벽에 갇힌 경우
                if (!isHealer)
                {
                    if (!isShoot)
                    {
                        isShoot = true;
                        currentState = EnemyState.Attack;
                    }
                }
                else {//힐러인 경우 => 공격안함. 그냥 TargetWall로 붙기.
                    transform.parent.transform.position = Vector3.MoveTowards(enemyAI.transform.position, targetWall.transform.position, 0.01f);
                }
                break;
            case 2://주변에 용병 있는 경우
                if (!isHealer)
                {
                    if (!isShoot)
                    {
                        isShoot = true;
                        currentState = EnemyState.Attack;
                    }
                }
                else
                {//힐러인 경우 => 주변 다친 애 있으면 힐. => 나중에

                }
                break;
            case 3:
                if (currentState == EnemyState.Attack)
                    return;

                enemyAI.enabled = true;
                currentState = EnemyState.Walk;
                enemyAI.SetDestination(dest);
                transform.parent.transform.position = Vector3.MoveTowards(enemyAI.transform.position, targetSecret.transform.position, 0.01f);
                break;
            case 4:
                if (currentState == EnemyState.Attack)
                    return;

                enemyAI.enabled = true;
                currentState = EnemyState.Walk;
                enemyAI.SetDestination(dest);
                transform.parent.transform.position = Vector3.MoveTowards(enemyAI.transform.position, OriginalPoint.transform.position, 0.01f);
                break;
        }
        SetAIDestination();
        Distance = Vector3.Distance(start, dest);
        if (Distance <= StopDistance) {
            IsArrived();
        }

        /*if (Distance <= StopDistance)
        {//목적지에 도달할 경우(목적지: base / Secret / Wall / Trap 중 1)

            if (isSurrounded)
            {
                if (targetWall != null)
                {
                    if (Vector3.Distance(dest, new Vector3(NavObj.transform.position.x, 0, NavObj.transform.position.z)) <= StopDistance)
                        Debug.Log(name + " reached " + targetWall.name);
                }
                else
                {
                    Debug.Log("IsSurrounded but No targetWall");
                }
            }

            if (isSeizure)
            {
                if (targetSecret != null)
                {
                    if (Vector3.Distance(dest, new Vector3(NavObj.transform.position.x, 0, NavObj.transform.position.z)) <= StopDistance) //Secret 도착
                        StartCoroutine(StealEvent());
                    else
                    {
                        enemyAI.enabled = true;
                        currentState = EnemyState.Walk;
                        enemyAI.SetDestination(new Vector3(OriginalPoint.transform.position.x, 0, OriginalPoint.transform.position.z));
                    }
                }
            }
            else if (dest == OriginalPoint.position)
            { //base 도착
                //탈출 이벤트 동작(골드 안주는거 빼곤 DieEvent와 동일)
                StartCoroutine(EscapeEvent());
            }
            //targetFriend 도착은 이미 위에서 처리
        }
        
        if (isSurrounded) {
            if (targetWall != null)
            {
                if (DirDistance(targetWall.gameObject, (int)ObjectType.Building))
                {
                    Debug.Log(name + ": Surround and targetWall exists Near.");
                    enemyAI.enabled = false;

                    if (!isShoot)
                    {
                        isShoot = true;
                        currentState = EnemyState.Attack;
                    }
                }
                else
                {
                    Debug.Log(name + ": Surrouded but too far to targetWall");
                    if (currentState == EnemyState.Attack)
                        return;

                    enemyAI.enabled = true;
                    currentState = EnemyState.Walk;
                    transform.parent.transform.position = Vector3.MoveTowards(enemyAI.transform.position, targetWall.transform.position, 0.01f);
//                    enemyAI.SetDestination(new Vector3(targetWall.transform.position.x, 0, targetWall.transform.position.z));
                }
            }
            else {
                transform.parent.transform.position = Vector3.MoveTowards(enemyAI.transform.position, dest, 0.01f);
            }
            //else {//벽은 없는데 경로가 없는 경우(있는진 모르겠는데 예외처리용)
            //    Debug.Log("Error Case: Is Surrounded is true but there's no wall around this character: "+name);
            //    currentState = EnemyState.Die;
            //    StartCoroutine(DieEvent());
            //}
        }

        else if (targetFriend != null)
        {
            if (DirDistance(targetFriend.gameObject, (int)ObjectType.Friendly))
            {
                enemyAI.enabled = false;

                if (!isShoot)
                {
                    isShoot = true;
                    currentState = EnemyState.Attack;
                }
            }
            if (!DirDistance(targetFriend.gameObject, (int)ObjectType.Friendly))
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

        */
    }
    protected void ChangeAnimation()
    {
        anime.SetInteger("Action", (int)currentState);
    }

    private void Awake()
    {
        isSurrounded = false;
        targetWall = null;
        targetTrap = null;
        NearFriendly = new List<Friendly>();
        NearWall = new List<Wall>();
        NearTrap = new List<Trap>();
        isShoot = false;
        enemyAI = GetComponentInParent<NavMeshAgent>();
        scollider = GetComponent<SphereCollider>();
        anime = GetComponent<Animator>();
        UIEnemyHealth = GetComponentInParent<HealthSystem>();
    }

    private void Update()
    {
        if (isDie)
            return;
        SetOrder();
        IsDeadEnd();
        EnemyAction();
        ChangeAnimation();
    }

    private void OnTriggerExit(Collider col) {
        if (col.CompareTag("Wall")) {
            Wall w=col.GetComponent<Wall>();
            if (NearWall.Contains(w)) {
                NearWall.Remove(w);
            }
        }
    }

    private void OnTriggerEnter(Collider col) {
        if (col.CompareTag("Friendly"))
        {
            Friendly e = col.GetComponent<Friendly>();
            if(!NearFriendly.Contains(e))
                NearFriendly.Add(e);
        }

        if (col.CompareTag("Wall")) {
            Wall w = col.GetComponentInParent<Wall>();
            if(!NearWall.Contains(w))
                NearWall.Add(w);
        }
    }
    private void SetOrder() {
        SetOrder_Wall();
        SetOrder_Trap();
        SetOrder_Friendly();
    }

    private void SetOrder_Trap() {
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
    private void SetOrder_Wall() {
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
        for (int i = 0; i < NearFriendly.Count; i++)
        {
            if (!NearFriendly[i].transform.parent.gameObject.activeSelf)
            {
                NearFriendly.Remove(NearFriendly[i]);
                i--;
            }
        }
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
            targetFriend= NearFriendly[0];
        }
        else
            targetFriend = null;
    }
    protected void IsDeadEnd() {
        if (enemyAI.pathStatus == NavMeshPathStatus.PathPartial || enemyAI.pathStatus == NavMeshPathStatus.PathInvalid)
        {
            isSurrounded = true;
        }
        else {
            isSurrounded = false;
        }
    }
}
