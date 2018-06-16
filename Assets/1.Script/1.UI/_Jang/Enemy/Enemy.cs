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
    Heal,
    Disassembly,
}
public class Enemy : MonoBehaviour {
    protected const string path = "Audio/Character/";
    public AudioSource Audio;
    public AudioClip[] Clips;
    public AudioClip[] DieClip;
    public string[] diaglogue=new string[4];//입장대사. 0, 1 -> 명예집단 대사, 2, 3 -> 기밀탈취단 대사
    public AudioClip AppearClip;
    public EnemyClusterManager ECM;
    public EnemyCluster myCluster;
    public EnemyGroup GroupConductor;       //캐릭터의 그룹을 정하고 정보를 받고 쓰기 위한 
    public HealthSystem UIEnemyHealth;      //현재 캐릭터의 체력을 나타내는 UI정보
    public Transform NavObj;                //NavMesh 상에서 움직이는 객체의 위치정보
    public Friendly targetFriend;           //타겟의 정보를 가져오기 위한
    public Transform OriginalPoint;			//기본 타겟을 저장하기위한
    public List<Wall> NearWall;
    public Wall targetWall;
    public Enemy healTarget;
    public SecretActs targetSecret;
    public List<Trap> NearTrap;
    public DayandNight DnN;
    public bool Movable;
    public bool isHealer;
    public bool isEntered;
    public int EnterOrder;
    public bool isLeft;
    public bool faceLeft;
    public bool isSurrounded;// 목적지(Secret / Base)까지의 경로가 막혔을 때 true
    public bool InnerSurrounded;//방 내부에서 Exitpoint까지의 경로가 막혔을 때 true
    public bool [] SubDestinations=new bool [7];
    /*
     * 0 : 건물
     * 1 : 적군
     * 2 : 아군
     * 3 : 함정
     * 4 : 기밀
     * 5 : Base
     * 6 : Exitpoint
     */
    public bool moveNextRoom;
    public int priority;
    public int Group;                       //4명으로 묶인 한 그룹. 그룹의 개수에 따라 1~3의 값을 가짐.
    public int Level;
    public int Hp;                          //체력
    public int Attack;                      //공격력
    public int MaxHp;                       //최대 체력
    public float Speed;                     //이동 속도
    public bool isDie;						//현재 죽은 상태 체크
    public bool isDefeated;                 // 거점 들어온 경우
    public bool isStolen;                 //기밀 들고 튄 경우
    public bool isSeizure;                  //탈취집단이면 true
    public int nextIdx;
    public int Exitdirection;
    public NavMeshAgent enemyAI;         //NevMeshAgent 
    protected Animator anime;               //애니메이션
    protected SphereCollider scollider;     //2D캐릭터에 붙어있는 콜라이더
    protected WaitForSeconds attackDelay;   //코루틴에서
    public EnemyState currentState;      //현재 캐릭터에 상태를 나타내는 
    protected EFFECT_TYPE effectType;       //캐릭터가 사용하는 이펙트 타입
    public int PresentRoomidx;
    public bool isShoot;                   //딜레이와 공격을 맞추기위한 
    protected bool isHeal;
    protected Vector3 PrevPos;
    public float Stoppingdistance;
    public NavMeshPathStatus TotalStatus, roomStatus, friendStatus;
    public bool roomSequenceEnter;
    public IEnumerator PrevroomMovement;
    protected NavMeshHit hitCheck;
    public bool isSample;
    public int CalPathMode;

    public Vector3 start;                //시작 좌표

    public Vector3 dest;                 //목적지 좌표(Base / secret만 저장)
    public GameObject DestObj;
    public string destname = "";

    public Vector3 roomDest;                 //목적지 좌표(Exitpoint만 저장)
    public GameObject roomDestObj;
    public string roomDestname = "";

    public Vector3 friendDest;                 //목적지 좌표(Friendly만 저장)
    public GameObject friendDestObj;
    public string friendDestname = "";

    public Vector3 wallDest;                 //목적지 좌표(Wall만 저장)
    public GameObject wallDestObj;
    public string wallDestname = "";

    public NavMeshPath navpathTotal;
    public NavMeshPath navpathRoom;
    public NavMeshPath navpathFriend;
    
    public float distanceTotal, distanceRoom, distanceFriend, distanceWall;                  //거리 체크 


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
            Attack = (int)Mathf.Ceil(Attack * 0.9f);
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
                for (int i = 0; i < myCluster.eList.Count; i++) {
                    myCluster.eList[i].Movable = true;
                }
                //EnemyActionMain();
            }
        }
        else if (targetWall)
        {
            targetWall.GetDamaged(Attack);
            GameManager.ParticleGenerate(effectType, targetWall.transform.position);
            if (targetWall.IsDestroyed())
            {
                NearWall.Remove(targetWall);
                StartCoroutine(BreakWall(targetWall));
                targetWall.DestoryWall();
                for (int i = 0; i < myCluster.eList.Count; i++)
                {
                    myCluster.eList[i].Movable = true;
                }
            }
        }
    }

    IEnumerator BreakWall(Wall w) {
        ECM.RemoveWall(w);        
        yield return null;

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
        myCluster.eList.Clear();
        yield return new WaitForSeconds(0.5f);
        SecretManager.SecretList.Remove(targetSecret);
        SecretManager.SecretCount--;
        if (targetSecret)
        {
            targetSecret.GetComponentInParent<DisplayObject>().DestroyObj(true);
            Destroy(targetSecret.transform.parent.gameObject);
        }
        yield return null;
        StartCoroutine(OriginalDestination());
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
        currentState = EnemyState.Walk;
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
        int random = Random.Range(0, AdjacentList.Count*2) % AdjacentList.Count;
        int result = AdjacentList[random];
        return result;
    }

    public int FindExit()
    {
        int result = 0;
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
        if (name.Equals("MonsterPickPocket2D")) {
            EnemyPickPocket epp = (EnemyPickPocket)this;
            if (!epp.IsDisassemble) { 
                if (epp.targetTrap && Vector3.Distance(dest, SetYZero(epp.targetTrap.transform)) < 0.1f)
                {                
                    StartCoroutine(epp.DisassemblyTrap());
                }
            }
            else {
                Actions();
            }
        }
        else { 
            Actions();
        }
    }

    protected void Actions()
    {
        if (Vector3.Distance(SetYZero(NavObj), SetYZero(OriginalPoint.transform)) <= enemyAI.stoppingDistance)
        {
            ClearEnemies();//Find all Enemies and Escape them.
        }
        else if (isSeizure && targetSecret && Vector3.Distance(SetYZero(NavObj), SetYZero(targetSecret.transform)) <= enemyAI.stoppingDistance)
        {
            StartCoroutine(StealEvent());
        }
        else if (myCluster.isGathered())
        {//next room 출구에 다 모인 경우
            myCluster.MoveToNextRoom(nextIdx, Exitdirection);
        }
        else {
            if (IsNearExit()) {
                enemyAI.isStopped = true;
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
        navpathTotal = new NavMeshPath();
        navpathRoom = new NavMeshPath();
        navpathFriend = new NavMeshPath();
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
        healTarget = null;
        NearWall = new List<Wall>();
        isShoot = false;
        enemyAI = GetComponentInParent<NavMeshAgent>();
        scollider = GetComponent<SphereCollider>();
        anime = GetComponent<Animator>();
        UIEnemyHealth = GetComponentInParent<HealthSystem>();
    }

    protected void Update()
    {
        if (enemyAI.isOnNavMesh) { 
            if (Movable)
            {
                enemyAI.isStopped = false;
            }
            else
                enemyAI.isStopped= true;
        }
        SetStart();

        if (friendDestObj && IsNear(NavObj, friendDestObj.transform))
        {
            if (!isShoot)
            {
                isShoot = true;
                Movable = false;
                currentState = EnemyState.Attack;
            }
        }
        else if (wallDestObj && roomStatus != NavMeshPathStatus.PathComplete) {
            distanceWall = Vector3.Distance(SetYZero(NavObj), wallDest);
            if (name == "MonsterFlyTeen2D" || name == "MonsterIDEFarmer2D" || name == "MonsterPickPocket2D")
            {
                if (distanceWall <= enemyAI.stoppingDistance * 2)
                {
                    if (!isShoot)
                    {
                        isShoot = true;
                        Movable = false;
                        currentState = EnemyState.Attack;
                    }
                }
            }
            else
            {
                if (distanceWall <= enemyAI.stoppingDistance)
                {
                    if (!isShoot)
                    {
                        isShoot = true;
                        Movable = false;
                        currentState = EnemyState.Attack;
                    }
                }
            }
        }
        else if (SubDestinations[5] && targetSecret && IsNear(NavObj, targetSecret.transform)) {
            StartCoroutine(StealEvent());
        }
        else if (SubDestinations[5] && IsNear(NavObj, OriginalPoint)) {
            //Base에 닿는 경우
            ECM.EscapeClusters();
        }
        else if (roomDestObj == GameManager.current.enemyGroups[PresentRoomidx].ExitPoint[Exitdirection].gameObject)
        {
            if (myCluster.isGathered())
            {//next room 출구에 다 모인 경우
                Debug.Log("All gathered");
                moveNextRoom = true;

                int nextroomEnterPoint = -1;
                if (Exitdirection == 1)
                {
                    nextroomEnterPoint = 3;
                }
                else
                {
                    nextroomEnterPoint = Mathf.Abs(Exitdirection - 2);
                }

                if (enemyAI.isOnNavMesh) { 
                    enemyAI.SetDestination(SetYZero(GameManager.current.enemyGroups[nextIdx].transform.GetChild(10).transform.GetChild(10)));
                    enemyAI.isStopped = false;
                }
                //transform.parent.transform.position = Vector3.MoveTowards(enemyAI.transform.position, SetYZero(GameManager.current.enemyGroups[nextIdx].ExitPoint[nextroomEnterPoint]), 0.01f);
                //                myCluster.MoveToNextEnter(nextIdx, Exitdirection);
            }
            else
            {
                Debug.Log("Else case");
                if (IsNearExit())
                {
                    Debug.Log("IsNearExit()");
                    enemyAI.isStopped = true;
                }
            }
        }
        ChangeAnimation();

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

    private void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Wall"))
        {
            Wall w = col.GetComponentInParent<Wall>();
            if (myCluster.GroupNearWall.Contains(w))
            {
                myCluster.GroupNearWall.Remove(w);
                myCluster.SetOrderWall();
            }
        }

        if (col.CompareTag("FriendlyBody")) {
            Friendly f = col.GetComponentInParent<Friendly>();

            if (myCluster.GroupNearFriend.Contains(f)) {
                myCluster.GroupNearFriend.Remove(f);
                myCluster.GetPriorFriend();
            }
        }
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
            if (!myCluster.GroupNearWall.Contains(w) && !w.isBraek) { 
                myCluster.GroupNearWall.Add(w);
                myCluster.SetOrderWall();
            }
        }
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
                if (!myCluster.GroupNearTrap.Contains(t)) { 
                    myCluster.GroupNearTrap.Add(t);
                    myCluster.SetOrderTrap();
                }
            }
            else
            {
                t = s.GetComponent<Trap>();
                if (myCluster.GroupNearTrap.Contains(t))
                {
                    myCluster.GroupNearTrap.Remove(t);
                    myCluster.SetOrderTrap();
                }
            }
        }
    }
    
    protected void SetDestination()
    {
        if (targetFriend) {
            dest = SetYZero(targetFriend.transform);
            destname = targetFriend.transform.parent.name;
            DestObj = targetFriend.gameObject;
        }
        else if (isSeizure && targetSecret)
        {
            dest = SetYZero(targetSecret.transform);
            destname = targetSecret.name;
            DestObj = targetSecret.gameObject;
        }
        else
        {
            dest = SetYZero(OriginalPoint.transform);
            destname = "OriginalPoint";
            DestObj = OriginalPoint.gameObject;
        }
        
        if (isHealer)
        {
            Enemy e;
            if ((e = myCluster.HurtEnemy()))
            {
                healTarget = e;
                dest = SetYZero(healTarget.NavObj);
                destname = healTarget.transform.parent.name;
                DestObj = healTarget.gameObject;
            }
        }

        if (name.Equals("MonsterPickPocket2D")) {
            if (isEntered) {
                NearTrap= GameManager.current.enemyGroups[PresentRoomidx].Traps;
            }
            EnemyPickPocket epp = (EnemyPickPocket)this;
            if (NearTrap.Count > 0)
            {
                epp.targetTrap = NearTrap[0];
                if (epp.targetTrap)
                {
                    dest = SetYZero(epp.targetTrap.transform);
                    destname = epp.targetTrap.name;
                    DestObj = epp.targetTrap.gameObject;
                }
            }
            else
            {
                epp.targetTrap = null;
            }
        }
    }

    
/*    protected void GroupActionMain()
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
    */
    protected virtual IEnumerator GiveHeal() { yield return null; }

    public void EnemyActionStart()
    {
        //StartCoroutine(EnemyAction());
        EnemyActionMain();
    }

    protected bool CalPath()
    {
        isSample = false;
        while (true) {
            if (enemyAI.isOnNavMesh)
            {
                NavMeshPath navpath=new NavMeshPath();
                enemyAI.CalculatePath(dest, navpath);
                enemyAI.SetPath(navpath);
                if (NavMesh.SamplePosition(dest, out hitCheck, 0.25f, NavMesh.AllAreas))
                {
                    isSample = true;
                }
                else
                {
                    isSample = false;
                }
                if (CalPathMode == 1)
                {
                    TotalStatus = navpath.status;
                    if (navpath.status != NavMeshPathStatus.PathComplete)
                    {
                        roomSequenceEnter = true;
                    }
                }
                else if (CalPathMode == 2) {
                    roomStatus = navpath.status;
                }
                Debug.Log("path calculated. status: "+enemyAI.pathStatus+ " isSample: " + isSample+" navpath: "+navpath.status);
                return true;
            }
        }

    }
    
    protected void SetDestination2nd() {
        Exitdirection = FindExit();
        if (targetFriend)
        {
            dest = SetYZero(targetFriend.transform);
            destname = targetFriend.transform.parent.name;
            DestObj = targetFriend.gameObject;
        }
        else if (targetWall) {
            dest = SetYZero(targetWall.transform);
            destname = targetWall.name;
            DestObj = targetWall.gameObject;
        }
        else if (PresentRoomidx >= 0 && PresentRoomidx <= 24)
        {
            dest = SetYZero(GameManager.current.enemyGroups[PresentRoomidx].ExitPoint[Exitdirection]);
            destname = "ExitPoint " + Exitdirection;
            DestObj = GameManager.current.enemyGroups[PresentRoomidx].ExitPoint[Exitdirection].gameObject;
        }

        if (isHealer)
        {
            Enemy e;
            if ((e = myCluster.HurtEnemy()))
            {
                healTarget = e;
                dest = SetYZero(healTarget.NavObj);
                destname = healTarget.transform.parent.name;
            }
        }

        if (name.Equals("MonsterPickPocket2D"))
        {
            if (isEntered)
            {
                NearTrap = GameManager.current.enemyGroups[PresentRoomidx].Traps;
            }
            EnemyPickPocket epp = (EnemyPickPocket)this;
            if (NearTrap.Count > 0)
            {
                epp.targetTrap = NearTrap[0];
                if (epp.targetTrap) { 
                    dest = SetYZero(epp.targetTrap.transform);
                    destname = epp.targetTrap.name;
                }
            }
            else
                epp.targetTrap = null;
        }
    }

    protected virtual IEnumerator EnemyAction()
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
                Debug.Log("Entering 1st lock");
                SetDestination2nd();
                yield return new WaitUntil(CalPath);
                if (isDie || isStolen || isDefeated)
                    break;
                if (enemyAI.pathStatus == NavMeshPathStatus.PathInvalid || enemyAI.pathStatus == NavMeshPathStatus.PathPartial)
                {
                    Debug.Log("Entering 2nd lock");
                    if (targetFriend && IsNear(NavObj, targetFriend.transform))
                    {
                        if (!isShoot)
                        {
                            currentState = EnemyState.Attack;
                            isShoot = true;
                            enemyAI.isStopped = true;
                        }
                    }
                    else if (targetWall && IsNear(NavObj, targetWall.transform))
                    {
                        if (!isShoot)
                        {
                            currentState = EnemyState.Attack;
                            isShoot = true;
                            enemyAI.isStopped = true;
                        }
                    }
                    else if (targetFriend) {
                        enemyAI.SetDestination(SetYZero(targetFriend.transform));
                        if (!Movable)
                        {
                            enemyAI.isStopped = true;
                            currentState = EnemyState.Idle;
                        }
                        else {
                            enemyAI.isStopped = false;
                            currentState = EnemyState.Walk;
                        }
                    }
                    else if (targetWall) {
                        enemyAI.SetDestination(SetYZero(targetWall.transform));
                        if (!Movable)
                        {
                            enemyAI.isStopped = true;
                            currentState = EnemyState.Idle;
                        }
                        else
                        {
                            enemyAI.isStopped = false;
                            currentState = EnemyState.Walk;
                        }
                    }
                    else
                    {
                        transform.parent.transform.position = Vector3.MoveTowards(enemyAI.transform.position, dest, 0.02f);
                        currentState = EnemyState.Walk;
                        enemyAI.isStopped = true;
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
                else if (targetWall && IsNear(NavObj, targetWall.transform)) {
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
                    if (!Movable)
                        enemyAI.isStopped = true;
                    else
                        enemyAI.isStopped = false;

                    currentState = EnemyState.Walk;
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
                if (!Movable)
                    enemyAI.isStopped = true;
                else
                    enemyAI.isStopped = false;

                currentState = EnemyState.Walk;
            }

            enemyAI.stoppingDistance = Stoppingdistance;
            ChangeAnimation();

            distanceTotal = Vector3.Distance(start, dest);
            if (distanceTotal <= enemyAI.stoppingDistance)
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

    protected void OnEnable() {
        targetSecret = FindClosestSecret(transform.position);
    }

    protected void SetOriginalDestination()
    {
        if (isSeizure && targetSecret)
        {
            dest = SetYZero(targetSecret.transform);
            destname = targetSecret.name;
            DestObj = targetSecret.gameObject;
        }
        else
        {
            dest = SetYZero(OriginalPoint.transform);
            destname = "OriginalPoint";
            DestObj = OriginalPoint.gameObject;
        }
    }

    public void EnemyActionMain() {
        TotalStatus = NavMeshPathStatus.PathComplete;
        roomStatus = NavMeshPathStatus.PathComplete;
        friendStatus = NavMeshPathStatus.PathComplete;
        FollowerInit();
//        StartCoroutine(OriginalDestination());
    }
    protected IEnumerator OriginalDestination() {
        SetStart();
        SetOriginalDestination();
        roomSequenceEnter = false ;
        Debug.Log("Originaldestination calpath");
        CalPathMode = 1;
        yield return new WaitUntil(CalPath);
        if (PrevroomMovement != null) {
            StopCoroutine(PrevroomMovement);
        }
        PrevroomMovement = roomMovement();
        StartCoroutine(PrevroomMovement);
        if (enemyAI.isOnNavMesh)
        {
            Debug.Log("go to destination");
            enemyAI.SetDestination(dest);
            currentState = EnemyState.Walk;
        }
        else {
            Debug.Log("enemyAi is not on navmesh");
        }
    }

    protected IEnumerator roomMovement()
    {
        while ( !(isDie || isDefeated || isStolen) )
        {
            if (roomSequenceEnter)
            {
                Debug.Log("Exitpoint Entered");
                Exitdirection = FindExit();
                dest = SetYZero(GameManager.current.enemyGroups[PresentRoomidx].ExitPoint[Exitdirection]);
                destname = "ExitPoint " + Exitdirection;
                DestObj = GameManager.current.enemyGroups[PresentRoomidx].ExitPoint[Exitdirection].gameObject;
                CalPathMode = 2;
                yield return new WaitUntil(CalPath);
                roomSequenceEnter = false;
                if (enemyAI.isOnNavMesh) {
                    enemyAI.SetDestination(dest);
                    currentState = EnemyState.Walk;
                }
                Debug.Log("roomstatus: " + roomStatus);
                if (roomStatus != NavMeshPathStatus.PathComplete && targetWall)
                {
                    Debug.Log("ChaseWall Activated");
                    //chaseWallEvent();
                }
            }

            if (roomStatus != NavMeshPathStatus.PathComplete && !IsNear(NavObj, DestObj.transform)) {
//                transform.parent.transform.position = Vector3.MoveTowards(enemyAI.transform.position, dest, 0.01f);
                currentState = EnemyState.Walk;
            }

            yield return null;//프레임마다 호출
        }
    }

    public void chaseFriendEvent() {
        StartCoroutine(chaseFriendly());
    }

    public void chaseWallEvent() {
        StartCoroutine(chaseWall());
    }

    public void chaseTrapEvent() {
        StartCoroutine(chaseTrap());
    }

    protected IEnumerator chaseFriendly()
    {
        if (!targetFriend)
            yield break;
        dest = SetYZero(targetFriend.transform);
        destname = targetFriend.name;
        DestObj = targetFriend.gameObject;
        Debug.Log("chaseFriendly Calpath");
        yield return new WaitUntil(CalPath);

        if (enemyAI.isOnNavMesh) { 
            if (enemyAI.pathStatus== NavMeshPathStatus.PathComplete)
            {
                enemyAI.SetDestination(dest);
                currentState = EnemyState.Walk;   
            }
            else
            {//벽으로 막혀서 못가는 경우
                Debug.Log("Chase friendly: wall break case");
                StartCoroutine(chaseWall());
            }
        }
    }

    protected IEnumerator chaseWall() {
        dest = SetYZero(targetWall.transform);
        destname = targetWall.name;
        DestObj = targetWall.gameObject;
        Debug.Log("chaseWall Calpath");
        yield return new WaitUntil(CalPath);
        Debug.Log("chase Wall status: " + enemyAI.pathStatus);
        if (enemyAI.isOnNavMesh) { 
            enemyAI.SetDestination(dest);
            currentState = EnemyState.Walk;
        }
        else
        {
            Debug.Log("chaseWall : Not on navmesh");
        }
    }

    protected IEnumerator chaseTrap() {
        EnemyPickPocket EP = (EnemyPickPocket)this;
        dest = SetYZero(EP.targetTrap.transform);
        destname = EP.targetTrap.name;
        DestObj = EP.targetTrap.gameObject;
        Debug.Log("chaseTrap Calpath");
        yield return new WaitUntil(CalPath);
        if (enemyAI.isOnNavMesh) {
            enemyAI.SetDestination(dest);
            currentState = EnemyState.Walk;
        }
    }

    protected void NearAction() {
        try
        {
            Debug.Log("NearAction Starts");
            Debug.Log("DestObj: "+DestObj.name);
        }
        catch { }
        if (isSeizure && (DestObj.name.Equals("AlienBloodStorage" )|| DestObj.name.Equals("SpaceVoiceRecordingFile") || DestObj.name.Equals("UFOCore") || DestObj.name.Equals("AlienStorageCapsule")))
        {
            StartCoroutine(StealEvent());
            return;
        }
        else if (DestObj.layer==14)//Base의 경우
        {
            ECM.EscapeClusters();
            return;
        }
        else if (name.Equals("MonsterPickPocket2D"))
        {
            EnemyPickPocket epp = (EnemyPickPocket)this;
            if (epp.targetTrap == DestObj)
            {
                if (!epp.IsDisassemble)
                {
                    StartCoroutine(epp.DisassemblyTrap());
                }
            }
        }
        else if (isHealer)
        {
            EnemySinger ES = (EnemySinger)this;

            if (!ES.isHeal)
            {
                currentState = EnemyState.Heal;
                enemyAI.isStopped = true;
                isHeal = true;
                StartCoroutine(ES.GiveHeal());
            }
        }
        else if (friendDestObj.name.Equals("Human"))
        {
            if (!isShoot)
            {
                isShoot = true;
                Movable = false;
                currentState = EnemyState.Attack;
            }
        }
        else if (DestObj.name.Equals("OldBuilding") || DestObj.name.Equals("NewBuilding") || DestObj.name.Equals("FunctionalBuilding") || DestObj.name.Equals("CoreBuilding"))
        {
            if (!isShoot)
            {
                isShoot = true;
                Movable = false;
                currentState = EnemyState.Attack;
            }
        }
        else if (DestObj == GameManager.current.enemyGroups[PresentRoomidx].ExitPoint[Exitdirection].gameObject) {
            if (myCluster.isGathered())
            {//next room 출구에 다 모인 경우
                myCluster.MoveToNextEnter(nextIdx, Exitdirection);
            }
            else
            {
                if (IsNearExit())
                {
                    enemyAI.isStopped = true;
                }
            }
        }
    }

    public bool IsNearExit() {
        bool result = Vector3.Distance(GetComponentInChildren<EnemyBody>().transform.position, GameManager.current.enemyGroups[PresentRoomidx].ExitPoint[Exitdirection].transform.position) > 0.3f;
        return (result)? false:true;
    }


    protected bool CalculatePath()
    {
        while (true)
        {
            if (enemyAI.isOnNavMesh)
            {
                switch (CalPathMode) {
                    case 1:
                        enemyAI.CalculatePath(dest, navpathTotal);
                        TotalStatus = navpathTotal.status;
                        break;
                    case 2:
                        enemyAI.CalculatePath(roomDest, navpathRoom);
                        roomStatus = navpathRoom.status;
                        break;
                    case 3:
                        enemyAI.CalculatePath(friendDest, navpathFriend);
                        friendStatus = navpathFriend.status;
                        break;
                }
                return true;
            }
        }

    }


    protected void totalDestination() {
        if (isSeizure && targetSecret)
        {
            dest = SetYZero(targetSecret.transform);
            destname = targetSecret.transform.parent.name;
            DestObj = targetSecret.gameObject;
        }
        else
        {
            dest = SetYZero(OriginalPoint.transform);
            destname = OriginalPoint.name;
            DestObj = OriginalPoint.gameObject;
        }
    }

    protected void roomDestination() {
        if (PresentRoomidx != 0)
        {
            roomDest = SetYZero(GameManager.current.enemyGroups[PresentRoomidx].ExitPoint[Exitdirection]);
            roomDestname = "ExitPoint " + Exitdirection;
            roomDestObj = GameManager.current.enemyGroups[PresentRoomidx].ExitPoint[Exitdirection].gameObject;
        }
        else {
            if (isSeizure && targetSecret)
            {
                roomDest = SetYZero(targetSecret.transform);
                roomDestname= targetSecret.transform.parent.name;
                roomDestObj= targetSecret.gameObject;
            }
            else
            {
                roomDest = SetYZero(OriginalPoint.transform);
                roomDestname = OriginalPoint.name;
                roomDestObj= OriginalPoint.gameObject;
            }
        }
    }

    protected void friendDestination() {
        friendDest = SetYZero(targetFriend.transform);
        friendDestname = targetFriend.transform.parent.name;
        friendDestObj = targetFriend.gameObject;
    }

    protected void FollowerInit() {
        StartCoroutine(FollowTotalPath());
        StartCoroutine(FollowRoomPath());
        StartCoroutine(FollowWall());
        StartCoroutine(FollowFriendly());
    }

    protected IEnumerator FollowTotalPath() {
        while ((!isDie && !isStolen && !isDefeated)) {
            if (moveNextRoom) {
                yield return null;
                continue;
            }

            totalDestination();
            CalPathMode = 1;
            yield return new WaitUntil(CalculatePath);
            
            if (TotalStatus == NavMeshPathStatus.PathComplete && (!SubDestinations[2]))
            {
                if (enemyAI.isOnNavMesh) { 
                    SubDestinations[5] = true;
                    currentState = EnemyState.Walk;
                    enemyAI.SetDestination(dest);
                    Debug.Log("totalpath setdest");
                }
            }
            else {
                SubDestinations[5] = false;
            }

            yield return null;
        }
    }
    
    protected IEnumerator FollowRoomPath()
    {
        Exitdirection = FindExit();
        while ((!isDie && !isStolen && !isDefeated))
        {
            if (moveNextRoom)
            {
                yield return null;
                continue;
            }
            if (isEntered)
            {
                Exitdirection = FindExit();
            }
            roomDestination();
            CalPathMode = 2;
            yield return new WaitUntil(CalculatePath);

            if (TotalStatus == NavMeshPathStatus.PathComplete || SubDestinations[2])
            {
                SubDestinations[6] = false;
            }
            else
            {
                CalPathMode = 2;
                yield return new WaitUntil(CalculatePath);

                if (!SubDestinations[0] && !SubDestinations[2])
                {
                    Debug.Log("Enter the gungeon. roomstatus: " + roomStatus);
                    if (roomStatus == NavMeshPathStatus.PathComplete)
                    {
                        if (enemyAI.isOnNavMesh)
                        {
                            Debug.Log("setdestination moving");
                            enemyAI.SetDestination(roomDest);
                        }
                    }
                    else {
                        Debug.Log("MoveToward moving");
                        transform.parent.transform.position = Vector3.MoveTowards(enemyAI.transform.position, roomDest, 0.03f);
                    }
                    currentState = EnemyState.Walk;
                    SubDestinations[6] = true;
                }
                else {
                    SubDestinations[6] = false;
                }
            }
            yield return null;
        }
    }

    protected IEnumerator FollowWall() {
        while ((!isDie && !isStolen && !isDefeated))
        {
            if (moveNextRoom)
            {
                yield return null;
                continue;
            }
            if (!targetWall)
            {
                wallDest = new Vector3(0, 0, 0);
                wallDestname = null;
                wallDestObj = null;
                SubDestinations[0] = false;
                yield return null;
            }
            else {
                wallDest = SetYZero(targetWall.transform);
                wallDestname = targetWall.name;
                wallDestObj = targetWall.gameObject;

                if (TotalStatus != NavMeshPathStatus.PathComplete && roomStatus != NavMeshPathStatus.PathComplete && !SubDestinations[2])
                {
                    if (enemyAI.isOnNavMesh)
                    {
                        SubDestinations[0] = true;
                        enemyAI.SetDestination(SetYZero(targetWall.transform));
                        currentState = EnemyState.Walk;
                        Debug.Log("wallpath setdest");
                    }
                }
                else {
                    SubDestinations[0] = false;
                }
            }
            yield return null;
        }
    }

    protected IEnumerator FollowFriendly()
    {
        while ((!isDie && !isStolen && !isDefeated))
        {
            if (moveNextRoom)
            {
                yield return null;
                continue;
            }
            if (!targetFriend)
            {
                friendDest = new Vector3 (0, 0, 0);
                friendDestname = null;
                friendDestObj = null;
                SubDestinations[2] = false;
                yield return null;
            }
            else
            {
                friendDestination();
                CalPathMode = 3;
                yield return new WaitUntil(CalculatePath);

                if (friendStatus == NavMeshPathStatus.PathComplete)
                {
                    SubDestinations[2] = true;
                    if (enemyAI.isOnNavMesh)
                    {
                        if (targetFriend) { 
                            enemyAI.SetDestination(SetYZero(targetFriend.transform));
                            currentState = EnemyState.Walk;
                            Debug.Log("friendpath setdest");
                        }
                    }
                }
                else
                {
                    SubDestinations[2] = false;
                }
            }
            yield return null;
        }
    }

}