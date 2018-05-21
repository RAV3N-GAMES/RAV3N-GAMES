using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGroup : MonoBehaviour {
    public int GroupIndex;
    public int GroupMemberMax;
    public bool isGenerate;
    public int isOpressed;//0: not opressed, 1: oppressed, 2: gold provided
    public int Count;
    /*
     * Genpoint
      - 0번 방 제외하고 모든 방에 4방향 전부 존재
      - 단, 초기 방(1, 5, 6)은 위치에 맞게 2개만 존재
      - 그 외의 경우 저장될 때 해당 정보 저장해주니 상관 X

     * Exitpoint
      - 모든 방에 4방향 존재
     */
    public List<Transform> GenPoint;
    public List<Transform> ExitPoint;
    List<Enemy> tmp;
    public List<Enemy> enemyList = new List<Enemy>();
    private WaitForSeconds genDelay = new WaitForSeconds(0.7f);

    private void Awake() {
        isOpressed = 0;
        Count = 0;
        foreach (Transform i in GenPoint)
        {
            Vector3 pos = new Vector3(i.position.x, 0, i.position.z);
            i.SetPositionAndRotation(pos, Quaternion.identity);
        }
    }
    private void OnEnable()
    {
        GameManager.EnemyGroupEvent += GetCurrentGroup;
    }
    private void OnDisable()
    {
        GameManager.EnemyGroupEvent -= GetCurrentGroup;
    }
    private EnemyGroup GetCurrentGroup()
    {
        return this;
    }
    private IEnumerator MemberAppear(float probability, int GroupId, int EnemyGroupIdx, int GenerateCount)
    {
        int Group_Normal = 0;//비행청소년, 사채업자, 부패경찰
        int Group_Special = 0;//음유시인, 분노조절장애농부, 소매치기

        if (GroupMemberMax <= 0)
            GroupMemberMax = 4;

        Enemy info = null;
        //Genopint =. GameManger에서 EnemyGroup으로 옮김
        int idx=-1;
        Transform pos = GenPoint[Random.Range(0, GenPoint.Count)];
        for (int i = 0; i < GroupMemberMax; ++i)
        {
            int rand = Random.Range(0, PoolManager.current.GetEnemyCountMax);

            if (rand == 1 && i == 0)
                rand = rand - 1;
            /*
             *  Group_Normal 및 Special을 2로 고정하는건 문제가 될 수 있음
             *  따라서 EnemyCount의 절반을 기준으로 하겠음.
             */
            if (Group_Normal >= GroupMemberMax / 2)
            {
                rand = Random.Range((int)(ENEMY_TYPE.Singer), PoolManager.current.GetEnemyCountMax);
            }
            else if (Group_Special >= GroupMemberMax / 2)
            {
                rand = Random.Range((int)(ENEMY_TYPE.FlyTeen), (int)(ENEMY_TYPE.Sheriff));
            }

            GameObject obj = PoolManager.current.PopEnemy((ENEMY_TYPE)rand);
            if (i == 0)
                idx = obj.GetComponentInChildren<Enemy>().CheckAdjacentCount();

            if (rand <= (int)(ENEMY_TYPE.Sheriff))
            {
                Group_Normal++;
            }
            else
            {
                Group_Special++;
            }

            obj.GetComponentInChildren<Enemy>().isSeizure = (SecretManager.criteria < probability) ? false : true;
            obj.GetComponentInChildren<Enemy>().Group = GroupId;
            obj.SetActive(false);
            obj.transform.position = new Vector3(pos.position.x, 0, pos.position.z);
            yield return genDelay;

            obj.transform.SetParent(null);
            info = obj.GetComponentInChildren<Enemy>();
            info.isEntered = true;
            info.OriginalPoint = GameManager.current.CommandPost;
            info.GroupConductor = this;
            info.UIEnemyHealth.HealthActvie(true);
            info.EnemyInit();
            info.PresentRoomidx = EnemyGroupIdx;
            info.myCluster = EnemyClusterManager.clusterList[GenerateCount];
            enemyList.Add(info);
            info.nextIdx = idx;
            EnemyClusterManager.clusterList[GenerateCount].eList.Add(info);
            obj.SetActive(true);
            obj.GetComponentInChildren<Enemy>().EnemyActionStart();
            GameManager.GenerateComplete = true;
        }
        Count++;
    }

    public void GroupMemberInit(float probability, int GroupId, int EnemygroupIdx, int GenerateCount)
    {
        StartCoroutine(MemberAppear(probability, GroupId, EnemygroupIdx, GenerateCount));
    }

    //적군이 죽었을 때 List에서 삭제
    public void RemoveEnemy(Enemy enemy)
    {
        DayandNight.DeadEnemy.Add(enemy);
        enemyList.Remove(enemy);
        GameManager.current.friendGroups[GroupIndex]
            .GroupRouteCall(GroupFindEnemy());
        /*GameManager.current.friendGroups[GroupIndex]
          	.FriendTargetChange(enemy);*/

        if (enemyList.Count <= 0)
        {
            enemyList.Clear();
            isGenerate = false;
        }
    }

    //그룹 안에서 살아있는 적을 받아오기 위한
    public Enemy GroupFindEnemy()
    {
        Enemy nextTarget = null;
        for (int i = 0; i < enemyList.Count; ++i)
        {
            if (!(enemyList[i].isDie || enemyList[i].isStolen || enemyList[i].isDefeated))
                nextTarget = enemyList[i];
        }

        return nextTarget;
    }

    //같은 그룹의 적의 타겟을 바꾸기위해 
    public void GroupRouteSet(Friendly friendly)
    {
        for (int i = 0; i < enemyList.Count; ++i)
        {
            enemyList[i].targetFriend = friendly;
        }
    }
    public void RangeInEnemy(Vector3 point, float damage, float dis,
        EFFECT_TYPE type)
    {
        Vector3 dest;
        Vector3 center;

        center = new Vector3(point.x, 0, point.z);
        for (int i = 0; i < enemyList.Count; ++i)
        {
            dest = new Vector3(enemyList[i].NavObj.position.x, 0,
                enemyList[i].NavObj.position.z);

            float rangeDis = Vector3.Distance(center, dest);
            if (rangeDis <= dis)
            {
                GameManager.ParticleGenerate(type, enemyList[i].NavObj.position);
                if (enemyList[i].Health((int)damage))
                {
                    enemyList[i].Die();
                }
            }
        }
    }

    public Enemy GetLessHpEnemy()
    {
        if (enemyList.Count <= 0)
            return null;

        int index = 0;
        int min = int.MaxValue;

        for (int i = 0; i < enemyList.Count; ++i)
        {
            if (min > enemyList[i].Hp)
            {
                min = enemyList[i].Hp;
                index = i;
            }
        }
        if (enemyList[index].MaxHp == enemyList[index].Hp)
            return null;
        else if (enemyList[index].MaxHp != enemyList[index].Hp)
            return enemyList[index];

        return null;
        //return enemyList[index];
    }

    public void ChildTriggered(Collider col) {
        if (col.CompareTag("EnemyBody")) {
            Enemy e = col.GetComponentInParent<Enemy>();
            if (enemyList.Contains(e)) { 
                enemyList.Remove(e);
            }
            else { 
                enemyList.Add(e);
                CountUp(e);
            }
        }
    }

    public void CountUp(Enemy e)
    {
        GameObject[] Enemies;
        Enemy CurE;
        int CurECount = 0;
        tmp = new List<Enemy>();
        Enemies = GameObject.FindGameObjectsWithTag("EnemyBody");
        Debug.Log("CountUp Call");
        Debug.Log(e.name + ".Group: " + e.Group);
        for (int i = 0; i < enemyList.Count; i++) {
            Debug.Log("enemyList[" + i + "].Group: " + enemyList[i].Group);
            if (enemyList[i].Group == e.Group) {
                tmp.Add(enemyList[i]);
            }
        }

        for (int i = 0; i < Enemies.Length; i++) {
            CurE = Enemies[i].GetComponentInParent<Enemy>();
            if (CurE.Group == e.Group)
                CurECount++;
        }

        Debug.Log("CurECount: " + CurECount);
        Debug.Log("tmp.Count: " + tmp.Count);
        if (CurECount == tmp.Count) {
            GroupEnter();
            Count++;
        }
    }

    public void GroupEnter() {
        int idx=0;
        if (tmp.Count > 0)
        {
            idx = tmp[0].CheckAdjacentCount();
        }
        for (int i = 0; i < tmp.Count; i++) {
            tmp[i].nextIdx = idx;
        }
    }
}