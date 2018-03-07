using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGroup : MonoBehaviour {

	public int GroupIndex;
	public int EnemyCount;
	public bool isGenerate;

	private List<Enemy> enemyList = new List<Enemy>();
	private WaitForSeconds genDelay = new WaitForSeconds(0.7f);

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
		if (EnemyCount <= 0)
			EnemyCount = 3;

		Enemy info = null;

		for (int i = 0; i < EnemyCount; ++i)
		{
			int rand = Random.Range(0, PoolManager.current.GetEnemyCountMax);

			if (rand == 1 && i == 0)
				rand = rand - 1;

			GameObject obj = PoolManager.current.PopEnemy((ENEMY_TYPE)rand);

			obj.SetActive(false);
			obj.transform.position = new Vector3(0, 0, 0);

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
		enemyList.Remove(enemy);
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
			if (enemyList[i].gameObject.activeSelf)
				return enemyList[i];
		}
		return null;
	}

	//같은 그룹의 적의 타겟을 바꾸기위해 
	public void GroupRouteSet(Friendly friendly)
	{
		for (int i = 0; i < enemyList.Count; ++i)
		{
			enemyList[i].Target = friendly.NavObj;
		}
	}

	public Enemy GetLessHpEnemy(Enemy myCall)
	{
		if (enemyList.Count <= 0)
			return null;

		int index = 0;
		int min = enemyList[0].Hp;

		for(int i = 1; i<enemyList.Count; ++i)
		{
			if (myCall == enemyList[i])
				continue;
			if (enemyList[i].MaxHp == enemyList[i].Hp)
				continue;

			if (min > enemyList[i].Hp)
			{
				min = enemyList[i].Hp;
				index = i;
			}
		}
		if (min == 0)
			return null;

		return enemyList[index];
	}

}
