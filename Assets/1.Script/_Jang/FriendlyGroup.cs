using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendlyGroup : MonoBehaviour {

	public int GroupIndex;
	public List<Friendly> friendList;

	public delegate void EnemyChange(Enemy enemy);
	public event EnemyChange EnemyChangeCall;

	public delegate Friendly OrderFind();
	public event OrderFind OrderCallEvent;

	private void Awake()
	{
		List<Friendly> friendList = new List<Friendly>();
	}
	private void OnEnable()
	{
		GameManager.FriendlyGroupEvent += GetGroup;
	}
	private void OnDisable()
	{
		GameManager.FriendlyGroupEvent -= GetGroup;
	}



	private FriendlyGroup GetGroup()
	{
		return this;
	}


	public void GroupInit(FRIEND_TYPE type, Transform pos)
	{
		GameObject obj = PoolManager.current.PopFriend(type);

		Friendly info = obj.GetComponentInChildren<Friendly>();

		info.GroupConductor = this;
		info.OriginalPoint = pos;

		//---------이 부분도 수정할 예정 
		OrderCallEvent += info.GetFriend;
		EnemyChangeCall += info.ChangeEnemyTarget;
		//---------------------------------------------

		obj.transform.SetParent(null);
		obj.SetActive(true);

		info.NavObj.position =
			new Vector3(pos.position.x, info.NavObj.position.y, pos.position.z);

		friendList.Add(info);
	}

	public void EnemyChangeEvent(Enemy enemy)
	{
		EnemyChangeCall(enemy);
	}

	public Friendly GetOrderFriendly()
	{ 
		int friendIndex = 0;
		int maxNumber= 0;
		
		for(int i=0; i<friendList.Count; ++i)
		{
			if (maxNumber < friendList[i].DamageStack)
			{
				maxNumber = friendList[i].DamageStack;
				friendIndex = i;
			}
		}

		if (friendList.Count <= 0)
			return null;

		if (OrderCallEvent().FriendType == FRIEND_TYPE.Resercher)
			return OrderCallEvent();
		else 
			return friendList[friendIndex];
		
	}
	public Friendly GetLessHpEnemy(Friendly myCall)
	{
		if (friendList.Count <= 0)
			return null;

		int index = 0;
		int min = friendList[0].Hp;

		for (int i = 1; i < friendList.Count; ++i)
		{
			if (myCall == friendList[i])
				continue;
			if (friendList[i].MaxHp >= friendList[i].Hp)
				continue;

			if (min > friendList[i].Hp)
			{
				min = friendList[i].Hp;
				index = i;
			}
		}
		if (min == 0)
			return null;

		return friendList[index];
	}
	//같은 그룹원들의 타겟을 바꾼다.
	public void GroupCall(Enemy target)
	{
		for(int i =0; i<friendList.Count; ++i)
		{
			if (friendList[i].targetEnemy != target)
				friendList[i].targetEnemy = target;
		}
	}

}
