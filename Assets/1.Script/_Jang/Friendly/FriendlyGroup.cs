using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendlyGroup : MonoBehaviour {

	public int GroupIndex;
	public List<Friendly> friendList;

	private delegate void EnemyTarget(Enemy enemy);
	private event EnemyTarget TargetChange;

	private void Awake()
	{
		List<Friendly> friendList = new List<Friendly>();
	}
	

	
	public void GroupInit(FRIEND_TYPE type, Transform pos)
	{
		GameObject obj = PoolManager.current.PopFriend(type);

		Friendly info = obj.GetComponentInChildren<Friendly>();

		info.GroupConductor = this;
		info.OriginalPoint = pos;
		TargetChange += info.TargetEnemyChange;
		obj.transform.SetParent(null);
		obj.SetActive(true);

		info.NavObj.position =
			new Vector3(pos.position.x, info.NavObj.position.y, pos.position.z);
		info.FriendlyInit();
		friendList.Add(info);	
	}
	public void FriendTargetChange(Enemy enemy)
	{
		if (enemy == null)
			return;

		TargetChange(enemy);
	}
	public void RemoveFriendly(Friendly data)
	{
		friendList.Remove(data);
		TargetChange -= data.TargetEnemyChange;

		GameManager.current.enemyGroups[GroupIndex].GroupRouteSet(GetOrderFriendly());

		if(friendList.Count <= 0)
		{
			friendList.Clear();
		}
	}
	public Friendly GetOrderFriendly()	//그룹원 주 타겟을 잡아주는
	{ 
		if (friendList.Count <= 0)
			return null;

		for (int i = 0; i < friendList.Count; ++i)
		{
			if (friendList[i].FriendType == FRIEND_TYPE.Resercher 
				&& !friendList[i].isDie)
				return friendList[i];
		}

		int friendIndex = 0;
		int maxNumber = 0;
		for (int i=0; i<friendList.Count; ++i)
		{
			if (maxNumber < friendList[i].DamageStack)
			{
				maxNumber = friendList[i].DamageStack;
				friendIndex = i;
			}
		}

		if (!friendList[friendIndex].isDie)
			return friendList[friendIndex];

		return friendList[0];
	}
	public Friendly GetLessHpFriendly()
	{
		if (friendList.Count <= 0)
			return null;

		int index = 0;
		int min = int.MaxValue;

		for (int i = 0; i < friendList.Count; ++i)
		{
			if (min > friendList[i].Hp)
			{
				min = friendList[i].Hp;
				index = i;
			}
		}

		if (friendList[index].Hp < friendList[index].MaxHp)
			return friendList[index];
		else
			return null;

	}		//HP가 제일 적은 그룹원을 넘김	
	public void GroupRouteCall(Enemy target)//그룹원 타겟이 없다면 타겟을 잡아줌
	{
		for(int i =0; i<friendList.Count; ++i)
		{
			if (friendList[i].targetEnemy == null)
				friendList[i].targetEnemy = target;
		}
	}
	public void GroupSpeedSet(int speed)
	{
		for(int i =0; i<friendList.Count; ++i)
		{
			friendList[i].AISpeed=  friendList[i].AISpeed * speed;
		}
	}	//그룹원 스피드 
	public Friendly RandomFriendly()
	{
		if (friendList.Count == 0)
			return null;

		int rand = Random.Range(0, friendList.Count + 1);
		return friendList[rand];
	}
}
