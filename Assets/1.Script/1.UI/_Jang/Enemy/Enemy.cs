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
public class Enemy : MonoBehaviour
{
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
    public DayandNight DnN;

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
        MaxHp = Hp;
        priority = -1;
    }

    public void HitAnimEnd()    //애니메이션에서 호출되는 함수
    {
        StartCoroutine(ShootEvent());
    }

    public void Hit()
    {
        if (targetFriend)
        {
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
        StartCoroutine("DieEvent");
    }
    public bool Health(int damage)
    {
        UIEnemyHealth.ValueDecrease(damage);
        if (Hp <= 10)
        {
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
    private void KillGold()
    {
        Data_Player.addGold(ResourceManager_Player.Tbl_Player[Data_Player.Fame - 4].Reward_Kill);
    }

    public void EscapeCoroutine()
    {
        StartCoroutine(EscapeEvent());
    }

    protected IEnumerator DieEvent()
    {
        Debug.Log("Die Event: " + name);
        isDie = true;
        isStolen = false;
        isDefeated = false;
        anime.SetTrigger("Die");
        enemyAI.enabled = false;
        scollider.enabled = false;
        UIEnemyHealth.HealthActvie(false);
        GroupConductor.RemoveEnemy(this);
        DayandNight.DeadEnemy.Add(this);
        yield return new WaitForSeconds(0.5f);
        transform.parent.gameObject.SetActive(false);
        PoolManager.current.PushEnemy(NavObj.gameObject);
    }

    protected IEnumerator EscapeEvent()
    {
        Debug.Log("Escape Event: " + name);
        isDie = false;
        isStolen = false;
        isDefeated = true;
        anime.SetTrigger("Die");
        enemyAI.enabled = false;
        scollider.enabled = false;
        UIEnemyHealth.HealthActvie(false);
        GroupConductor.RemoveEnemy(this);
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

    protected IEnumerator StealEvent()
    {
        Debug.Log(name + ": StealEvent Triggered");
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

        for (int i = 0; i < List.Count; i++)
        {
            List[i].isDie = false;
            List[i].isStolen = true;
            List[i].isDefeated = false;
            List[i].anime.SetTrigger("Die");
            List[i].enemyAI.enabled = false;
            List[i].scollider.enabled = false;
            List[i].UIEnemyHealth.HealthActvie(false);
            List[i].GroupConductor.RemoveEnemy(this);
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

        if (Type == (int)ObjectType.Friendly && targetFriend != null)
        { // 없어도??
            if (targetFriend.NavObj.position.x > NavObj.position.x)
                transform.localScale = new Vector3(-1, 0, 1);
            else
                transform.localScale = new Vector3(1, 0, 1);
        }
        //Debug.Log(name + " - Distance: " + Distance + " StopDistance: " + StopDistance+ " DirDistance: "+ ((Distance<StopDistance)? true : false).ToString());

        if (Type == (int)ObjectType.Building)
        {
            if (Distance < enemyAI.stoppingDistance * 3)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (Distance < enemyAI.stoppingDistance * 3)
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

    private bool IsNear(Transform NavObjPos, Transform targetPos)
    {
        return (Vector3.Distance(NavObjPos.position, targetPos.position) <= enemyAI.stoppingDistance) ? true : false;
    }

    private Vector3 SetYZero(Transform t)
    {
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
        int random = Random.Range(0, AdjacentList.Count - 1);
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

    private void SetStart()
    {
        start = new Vector3(NavObj.position.x, 0, NavObj.position.z);
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
         *  1.1 경로 없는 경우 다음 방 입구(Priority : 1)
         *  1.2 다음 방 입구까지 경로 없는 경우 (Priority: 6)
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

        if (e.name == "PickPocket" && targetTrap)
        {
            dest = SetYZero(targetTrap.transform);
            priority = 0;
            return;
        }

        if (!isHealer)
        {
            if (isSurrounded)
            {

            }

            else
            {
                if (isSeizure && (targetSecret = FindClosestSecret(start)))
                {
                    dest = SetYZero(targetSecret.transform);
                    priority = 3;
                }
                else
                {
                    dest = SetYZero(OriginalPoint.transform);
                    priority = 4;
                }
            }

            /*if (targetFriend && IsNear(NavObj.transform, targetFriend.transform))
            {//아군 용병이 공격범위 안에 있을 경우 dest는 용병
                dest = SetYZero(targetFriend.transform);
                priority = 2;
            }
            else if (isSurrounded)
            {
                nextIdx = CheckAdjacentCount();
                Exitdirection = FindExit();
                dest = SetYZero(GameManager.current.enemyGroups[PresentRoomidx].ExitPoint[Exitdirection]);

                if (targetWall && IsNear(NavObj.transform, targetWall.transform))
                {
                    
                    // * Count 고려해서 dest 선정
                    // *  1. 상하좌우 Count 최소값 확인
                    // *  2. 최소값인 enemyGroup 특정
                    // *  3. 섞어줌(필요없을수 있음)
                    // *  4. 0번째 인덱스로 전진 
                    // *      4.1 dest 는 0번째 인덱스로 가는 방문(해당 enemyGroup에 포인트를 정함)
                    // *       4.1.1 dest까지 가는 경로가 다시 막혀있으면(PathInnerRoomDisabled가 true이면) => 
                    // *             dest=targetWall
                    // *      4.2 살아있는 모든 팀원이 모인 후에 다음 방으로 건너감
                    // *       4.2.1 door를 포인트로 지정할 것이므로 거긴 막혀있지 않음.
                     
                    priority = 1;
                    //dest = SetYZero(targetWall.transform);
                }
                else
                {
                    priority = 5;//경로는 없는데 주변에 벽이 없는 경우
                    // => Count에 따라 옆 방으로 보냄
                }*/
        }
    }

    protected void SetAction()
    {
        if (priority == 1)
        {
            if (Vector3.Distance(start, dest) < enemyAI.stoppingDistance)
            {
                dest = SetYZero(GameManager.current.enemyGroups[nextIdx].ExitPoint[0]);
            }
        }
        switch (priority)
        {
            case 0:
                //소매치기가 함정 타게팅 했을 때
                break;
            case 1://Base / Secret까지의 경로가 없는 경우
                if (!isHealer)
                {
                    if (!isShoot)
                    {
                        //enemyAI.enabled = false;
                        enemyAI.isStopped = true;
                        isShoot = true;
                        currentState = EnemyState.Attack;
                    }
                }
                else
                {//힐러인 경우 => 공격안함. 그냥 TargetWall로 붙기.
                    //transform.parent.transform.position = Vector3.MoveTowards(enemyAI.transform.position, targetWall.transform.position, 0.01f);
                }
                break;
            case 2://주변에 용병 있는 경우
                if (!isHealer)
                {
                    //enemyAI.enabled = false;
                    enemyAI.isStopped = true;
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

                enemyAI.isStopped = false;
                currentState = EnemyState.Walk;

                //enemyAI.SetDestination(dest);
                break;
            case 4:
                if (currentState == EnemyState.Attack)
                    return;

                enemyAI.isStopped = false;
                currentState = EnemyState.Walk;
                //enemyAI.SetDestination(dest);
                break;
            case 5:
                enemyAI.isStopped = false;
                currentState = EnemyState.Walk;
                enemyAI.SetDestination(SetYZero(OriginalPoint));
                transform.parent.transform.position = Vector3.MoveTowards(enemyAI.transform.position, OriginalPoint.transform.position, 0.01f);
                break;
            case 6://Inner Surrounded 상태
                enemyAI.isStopped = false;
                currentState = EnemyState.Walk;
                Vector3 p = SetYZero(GameManager.current.enemyGroups[PresentRoomidx].ExitPoint[Exitdirection]);
                enemyAI.SetDestination(p);
                transform.parent.transform.position = Vector3.MoveTowards(enemyAI.transform.position, p, 0.01f);
                break;
        }
    }

    private void SetAIDestination()
    {
        switch (priority)
        {
            case 0:
                enemyAI.SetDestination(SetYZero(targetTrap.transform));
                break;
            case 1:
                if (enemyAI.enabled)
                    enemyAI.SetDestination(dest);
                break;
            case 2:
                if (!isHealer)
                {
                    //enemyAI.SetDestination(SetYZero(targetFriend.transform));
                }
                else
                {//다친 Enemy 한테로 destination 설정


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
        else
        {
            Debug.Log(name + "Nextidx: " + nextIdx);
            if (nextIdx == -1)
                return;
            Transform nextRoom = GameManager.current.enemyGroups[nextIdx].ExitPoint[Exitdirection];
            enemyAI.SetDestination(nextRoom.position);
        }
    }

    protected void Flip()
    {
        faceLeft = !faceLeft;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    private void EnemyAction()
    {
        //시작지 설정
        //목적지 설정(우선순위별)
        //행동 설정
        SetStart();
        SetDest(this);
        SetAction();
        SetAIDestination();
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
            if (transform.position.x - PrevPos.x > 0.05)
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

    protected void ChangeAnimation()
    {
        anime.SetInteger("Action", (int)currentState);
    }

    private void Awake()
    {
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
        //        IsDeadEnd();
        //        EnemyAction();
        //        ActionMain();
        GroupActionMain();
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
        if (col.CompareTag("Friendly"))
        {
            Friendly e = col.GetComponent<Friendly>();
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

    protected void GetTrap()
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
            targetFriend = NearFriendly[0];
        }
        else
            targetFriend = null;
    }
    protected void IsDeadEnd()
    {
        if (enemyAI.pathStatus == NavMeshPathStatus.PathPartial || enemyAI.pathStatus == NavMeshPathStatus.PathInvalid)
        {
            if (dest == SetYZero(OriginalPoint) || (targetSecret && dest == SetYZero(targetSecret.transform)))
                isSurrounded = true;
            else
                InnerSurrounded = true;
        }
        else
        {
            if (dest == SetYZero(OriginalPoint) || (targetSecret && dest == SetYZero(targetSecret.transform)))
                isSurrounded = false;
            else
                InnerSurrounded = false;
        }
    }


    protected void ActionMain()
    {
        SetStart();
        SetDestination();
        Debug.Log(name + " NavMeshstatus: " + enemyAI.pathStatus);
        if (targetFriend)
        {
            Debug.Log(name + "targetFriend Case start");
            enemyAI.SetDestination(SetYZero(targetFriend.transform));
            if (IsNear(NavObj, targetFriend.transform))
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
                enemyAI.isStopped = false;
                currentState = EnemyState.Walk;
            }
        }
        else if (enemyAI.pathStatus == NavMeshPathStatus.PathPartial || enemyAI.pathStatus == NavMeshPathStatus.PathInvalid)
        {//주 목적지까지의 경로가 없는 경우
            Debug.Log(name + "No path case start");
            nextIdx = CheckAdjacentCount();
            Exitdirection = FindExit();
            dest = SetYZero(GameManager.current.enemyGroups[PresentRoomidx].ExitPoint[Exitdirection]);
            enemyAI.SetDestination(dest);
            Debug.Log(name + "Directoin: " + Exitdirection);
            if (enemyAI.pathStatus == NavMeshPathStatus.PathPartial || enemyAI.pathStatus == NavMeshPathStatus.PathInvalid)
            {//다음 방으로의 길이 막힌 경우
                Debug.Log("Locked the door case");
                if (targetWall && IsNear(NavObj, targetWall.transform))//가까운 벽이 있으면 때려부순 다음 길 재측정
                {
                    Debug.Log("Smash the door case");
                    if (!isShoot)
                    {
                        enemyAI.isStopped = true;
                        isShoot = true;
                        currentState = EnemyState.Attack;
                    }
                }
                else
                {//가까운 벽이 없으면 최대한 목적지(출구)쪽으로 비빔
                    Debug.Log("Go to the door case");
                    currentState = EnemyState.Walk;
                    enemyAI.isStopped = true;
                    transform.parent.transform.position = Vector3.MoveTowards(enemyAI.transform.position, dest, 0.02f);
                }
            }
            else
            {//다음 방 까지는 갈 수 있는 경우 => 그냥 직진함.
                Debug.Log(name + "To next exit case start");
                currentState = EnemyState.Walk;
                enemyAI.isStopped = false;
            }
        }
        else
        {
            enemyAI.isStopped = false;
            currentState = EnemyState.Walk;
        }

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

    protected void SetDestination()
    {
        targetSecret = FindClosestSecret(NavObj.position);
        if (isSeizure && targetSecret)
        {
            dest = SetYZero(targetSecret.transform);
        }
        else
        {
            dest = SetYZero(OriginalPoint.transform);
        }
        enemyAI.SetDestination(dest);
    }

    protected void GroupActionMain()
    {
        SetStart();
        SetDestination();
        if (isEntered)
        {
            //해당 방의 Trap / Secret 받아오기
            Debug.Log(name + " PresentIdx: " + PresentRoomidx);
            NearTrap.Clear();
            GetTrap();
            targetSecret = FindClosestSecret(NavObj.transform.position);
            isEntered = false;
        }

        if (nextIdx != -1)
        {
            if (enemyAI.pathStatus == NavMeshPathStatus.PathInvalid || enemyAI.pathStatus == NavMeshPathStatus.PathPartial)
            {
                if (targetFriend && IsNear(NavObj, targetFriend.transform))
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
                            dest = SetYZero(GameManager.current.enemyGroups[PresentRoomidx].ExitPoint[Exitdirection]);
                    }
                }
                enemyAI.SetDestination(dest);

                if (enemyAI.pathStatus == NavMeshPathStatus.PathInvalid || enemyAI.pathStatus == NavMeshPathStatus.PathPartial)
                {
                    if (targetWall && IsNear(NavObj, targetWall.transform))
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
                    if (targetFriend && IsNear(NavObj, targetFriend.transform))
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
                        enemyAI.isStopped = false;
                        currentState = EnemyState.Walk;
                    }
                }
            }
            else
            {
                if (targetFriend && IsNear(NavObj, targetFriend.transform))
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
                    enemyAI.isStopped = false;
                    currentState = EnemyState.Walk;
                }
            }
        }
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
}
