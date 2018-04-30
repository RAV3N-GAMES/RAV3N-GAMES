using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ENEMY_TYPE
{
	FlyTeen,
	Mafia,
	Sheriff,	
	Singer,
	Farmer,
	Thief,
}
public enum FRIEND_TYPE
{
	Army,
	Guard,
	Chemistry,
	Resercher,
}
public enum EFFECT_TYPE
{
	Hit,
	Approach,
	Heal,
	Bang,
	Invisible,
}


public class PoolManager : MonoBehaviour {
    public static PoolManager current;
	public GameObject[] EnemyPrefabs;
	public GameObject[] FriendPrefabs;
	public GameObject[] ParticlePrefabs;

	private List<GameObject>[] EnemyPoolList;
	private List<GameObject>[] FriendPoolList;
	private List<GameObject>[] ParticlePoolList;

	public Transform characterParent;
	public Transform particleParent;


	public int SetCount;

	public int GetEnemyCountMax
	{
		get { return EnemyPrefabs.Length; }
	}
	public int GetFriendCountMax
	{
		get { return FriendPrefabs.Length; }
	}

	private void Awake()
	{
		if (current == null)
			current = this;
		else
			Destroy(gameObject);

		CharacterPoolInit();
		ParticlePoolInit();
	}
	private void CharacterPoolInit()
	{
		GameObject parent = new GameObject("Character");
		characterParent = parent.transform;
		characterParent.SetParent(transform);

		int count = EnemyPrefabs.Length;
		EnemyPoolList = new List<GameObject>[count];

		
		SetCount = 7;

		for (int i = 0; i < count; ++i)
		{
			EnemyPoolList[i] = new List<GameObject>();
			for (int j = 0; j < SetCount; ++j)
			{
				GameObject obj = Instantiate(EnemyPrefabs[i]);
				obj.SetActive(false);
				obj.name = EnemyPrefabs[i].name;
				obj.transform.SetParent(characterParent);
                obj.GetComponentInChildren<Enemy>().DnN = GameObject.Find("Curtain").GetComponent<DayandNight>();
				EnemyPoolList[i].Add(obj);
			}
		}

		count = FriendPrefabs.Length;
		FriendPoolList = new List<GameObject>[count];
		for (int i = 0; i < count; ++i)
		{
			FriendPoolList[i] = new List<GameObject>();
			for (int j = 0; j < SetCount; ++j)
			{
				GameObject obj = Instantiate(FriendPrefabs[i]);
				obj.SetActive(false);
				obj.name = FriendPrefabs[i].name;
				obj.transform.SetParent(characterParent);
				FriendPoolList[i].Add(obj);
			}
		}
	}
	private void ParticlePoolInit()
	{
		GameObject parent = new GameObject("Particle");
		particleParent = parent.transform;
		particleParent.SetParent(transform);

		int count = ParticlePrefabs.Length;
		ParticlePoolList = new List<GameObject>[count];

		GameObject poolObj;

		for (int i = 0; i < count; ++i)
		{
			ParticlePoolList[i] = new List<GameObject>();

			for (int j = 0; j < SetCount; ++j)
			{
				poolObj = Instantiate(ParticlePrefabs[i]);
				poolObj.name = ParticlePrefabs[i].name;
				poolObj.transform.SetParent(particleParent);
				poolObj.SetActive(false);

				ParticlePoolList[i].Add(poolObj);
			}
		}


	}
	public GameObject PopFriend(FRIEND_TYPE type)
	{
		if (FriendPoolList[(int)type].Count > 0)
		{
			GameObject obj = FriendPoolList[(int)type][0];
			obj.transform.SetParent(null);
			FriendPoolList[(int)type].RemoveAt(0);
			return obj;
		}
		else if (FriendPoolList[(int)type].Count <= 0)
		{
			GameObject obj = Instantiate(FriendPrefabs[(int)type]);
			obj.transform.SetParent(null);
			return obj;
		}
		return null;
	}
	public GameObject PopEnemy(ENEMY_TYPE type)    
	{
		if (EnemyPoolList[(int)type].Count > 0)
		{
			GameObject obj = EnemyPoolList[(int)type][0];
			obj.SetActive(true);

			obj.transform.SetParent(null);
			EnemyPoolList[(int)type].RemoveAt(0);
            DayandNight.CreatedEnemy.Add(obj.GetComponentInChildren<Enemy>());
			return obj;
		}
		else if (EnemyPoolList[(int)type].Count <= 0)
		{
            Debug.Log("Pop Enemy when Count under 0 in PoolManager");
            GameObject obj = Instantiate(EnemyPrefabs[(int)type]);
            obj.name = EnemyPrefabs[(int)type].name;
			obj.transform.SetParent(null);
			obj.SetActive(true);
            obj.GetComponentInChildren<Enemy>().DnN = GameObject.Find("Curtain").GetComponent<DayandNight>();
            DayandNight.CreatedEnemy.Add(obj.GetComponentInChildren<Enemy>());
            
            return obj;
		}
		return null;
	}
	public GameObject PopParticle(EFFECT_TYPE type)
	{
		if (ParticlePoolList[(int)type].Count > 0)
		{
			GameObject obj = ParticlePoolList[(int)type][0];
			obj.transform.SetParent(null);
			obj.SetActive(true);
			ParticlePoolList[(int)type].RemoveAt(0);
			return obj;
		}
		else if (ParticlePoolList[(int)type].Count <= 0)
		{
			GameObject obj = Instantiate(ParticlePrefabs[(int)type]);
			obj.name = ParticlePrefabs[(int)type].name;
			obj.transform.SetParent(null);
			obj.SetActive(true);
			return obj;
		}

		return null;
	}
	public void PushParticle(GameObject obj)       
	{
		for (int i = 0; i < ParticlePoolList.Length; ++i)
		{
			if (ParticlePrefabs[i].name.Equals(obj.name))
			{
				obj.SetActive(false);
				obj.transform.SetParent(particleParent);
				ParticlePoolList[i].Add(obj);
				return;
			}
		}
	}
	public void PushFriend(GameObject obj)         
	{
		for (int i = 0; i < FriendPoolList.Length; ++i)
		{
			if (FriendPoolList[i][0].name.Equals(obj.name))
			{
				obj.transform.SetParent(characterParent);
				obj.SetActive(false);
				FriendPoolList[i].Add(obj);
				return;
			}
		}

	}
	public void PushEnemy(GameObject obj)		   
	{
		for (int i = 0; i < EnemyPoolList.Length; ++i)
		{
			if (EnemyPoolList[i][0].name.Equals(obj.name))
			{
				obj.transform.SetParent(characterParent);
				obj.SetActive(false);
				EnemyPoolList[i].Add(obj);
				return;
			}
		}
	}
}
