using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGroup : MonoBehaviour {

	public int GroupIndex;
	public int EnemyCount;
	public bool isGenerate;
    public Transform[] GenPoint;

    private List<Enemy> enemyList = new List<Enemy>();
	private WaitForSeconds genDelay = new WaitForSeconds(0.7f);

    private void Awake() {
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
	private IEnumerator MemberAppear()
	{
        int Group_Normal = 0;//비행청소년, 사채업자, 부패경찰
        int Group_Special = 0;//음유시인, 분노조절장애농부, 소매치기
        
		if (EnemyCount <= 0)
			EnemyCount = 4;

		Enemy info = null;
        //Genopint =. GameManger에서 EnemyGroup으로 옮김
		Transform pos = GenPoint[Random.Range(0, GenPoint.Length)];
		for (int i = 0; i < EnemyCount; ++i)
		{
			int rand = Random.Range(0, PoolManager.current.GetEnemyCountMax);

			if (rand == 1 && i == 0)
				rand = rand - 1;
            /*
             *  Group_Normal 및 Special을 2로 고정하는건 문제가 될 수 있음
             *  따라서 EnemyCount의 절반을 기준으로 하겠음.
             */
            if (Group_Normal >= EnemyCount / 2)
            {
                rand = Random.Range((int)(ENEMY_TYPE.Singer), PoolManager.current.GetEnemyCountMax);
            }
            else if (Group_Special >= EnemyCount / 2)
            {
                rand = Random.Range((int)(ENEMY_TYPE.FlyTeen), (int)(ENEMY_TYPE.Sheriff));
            }
            GameObject obj = PoolManager.current.PopEnemy((ENEMY_TYPE)rand);
            if (rand <= (int)(ENEMY_TYPE.Sheriff))
            {
                Group_Normal++;
            }
            else
            {
                Group_Special++;
            }

            obj.SetActive(false);
			obj.transform.position = new Vector3(pos.position.x, 0, pos.position.z);

			yield return genDelay;

			obj.transform.SetParent(null);
			obj.SetActive(true);

			info = obj.GetComponentInChildren<Enemy>();
			info.OriginalPoint = GameManager.current.CommandPost;
			info.GroupConductor = this;
			info.UIEnemyHealth.HealthActvie(true);
			info.EnemyInit();
			enemyList.Add(info);
        }
	}

	public void GroupMemberInit()
	{
		StartCoroutine(MemberAppear());
	}
	

	
	//적군이 죽었을 때 List에서 삭제
	public void RemoveEnemy(Enemy enemy)
	{
        DayandNight.DeadEnemy.Add(enemy);
		enemyList.Remove(enemy);
        GameManager.current.friendGroups[GroupIndex]
            .GroupRouteCall(GroupFindEnemy());
//			.FriendTargetChange(enemy);

		if (enemyList.Count <= 0)
		{
			enemyList.Clear();
			isGenerate = false;
		}
	}

	//그룹 안에서 살아있는 적을 받아오기 위한
	public Enemy GroupFindEnemy()
	{
		for(int i =0; i<enemyList.Count; ++i)
		{
			if (!enemyList[i].isDie)
				return enemyList[i];
		}
		return null;
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
				if(enemyList[i].Health((int)damage))
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

		for(int i = 0; i<enemyList.Count; ++i)
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
	

}
