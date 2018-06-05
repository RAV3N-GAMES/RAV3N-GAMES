﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum DIRECTION
{
	RIGHT,
	LEFT,
}

public class GameManager : MonoBehaviour {
    public static GameManager current;
	public delegate EnemyGroup GetEnemyGroup();
	public static event GetEnemyGroup EnemyGroupEvent;
	public RoomManager roomManager;
	private EnemyGroup genGroup = null;
	public Transform GroupParent;
	public Transform CommandPost;	
	public EnemyGroup[]	enemyGroups;
	public FriendlyGroup[] friendGroups;
    public EnemyClusterManager ECM;
	public CubeObject getRayTrans;
	private LayerMask tileMask;
	private bool isFloorClick;
    public static int OppressedEnemy;
    public DayandNight Curtain;

    public static bool GenerateComplete;

	public int GetRoomIndex
	{
		get { return roomManager.CenterRoomIdx; }
	}
	private void Awake()
	{
        GenerateComplete = false;
        ECM = GameObject.Find("Managers").GetComponent<EnemyClusterManager>();
        if (current == null)
			current = this;
		else
			Destroy(gameObject);

		tileMask = LayerMask.GetMask("Tile");
		GroupInit();
        OppressedEnemy = 0;
    }
	private void Update()
	{
		
		RayEvent();
	}

	public void EnemyGenerate()
	{
        int i = 0;
        int GenerateCount = 0;
        List<EnemyGroup> activeGroup = new List<EnemyGroup>();
        for (i = 0; i < enemyGroups.Length; i++) {
            if (enemyGroups[i].gameObject.activeSelf) {
                activeGroup.Add(enemyGroups[i]);
            }
        }
        for(i=1;i<enemyGroups.Length;i++){//0번방에선 Enemy 생성 안함
            int random = Random.Range(1, activeGroup.Count);
            float probability = Random.Range(0f, 1f);

            genGroup = activeGroup[random];

            if (!genGroup.isGenerate)
            {
                genGroup.isGenerate = true;

                genGroup.GroupMemberInit(probability, i, genGroup.GroupIndex, GenerateCount);     //적 그룹을 생성하지않은 곳을 찾아 생성호출
                GenerateCount++;
            }

            if (GenerateCount == ResourceManager_Player.Tbl_Player[Data_Player.Fame - 4].enemyClusterNumber) {
                ActiveEnemyManager.IsCreated = true;
                break;
            }
        }
        EnemyManager.EnemyGroupMax = ResourceManager_Player.Tbl_Player[Data_Player.Fame - 4].enemyClusterNumber;
    }
	private void RayEvent()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		if(Input.GetMouseButton(0))
		{
			if (Physics.Raycast(ray, out hit, 100, tileMask))
			{
				if (hit.collider.CompareTag("Tile"))
				{
					if (!isFloorClick)
					{
						getRayTrans = hit.transform.GetComponent<CubeObject>();
						Debug.Log(getRayTrans.name);
						isFloorClick = true;
					}
				}
			}
		}
	}
	public void FriendTypeSelect(int index)
	{
		FRIEND_TYPE type = (FRIEND_TYPE)index;
		int roomIndex = roomManager.CenterRoomIdx;
		if (getRayTrans == null)
			return;

		int roomindex = roomManager.CenterRoomIdx;
        //roomindex 랑 roomIndex랑 차이점?? => 나중에 방 여러개 사용하게 함수 바꾸면 용도가 달라질지도?

		if (!friendGroups[roomindex].enabled)
			return;

		isFloorClick = false;
		friendGroups[roomIndex].GroupInit(type, getRayTrans.CubeTrans);
		getRayTrans = null;

	}

	private void GroupInit()
	{
		int count = GroupParent.childCount;
		enemyGroups = new EnemyGroup[count];
		friendGroups = new FriendlyGroup[count];

		for (int i = 0; i < count; ++i)
		{
			enemyGroups[i] = GroupParent.GetChild(i).GetComponent<EnemyGroup>();
			enemyGroups[i].GroupIndex = i;
			friendGroups[i] = GroupParent.GetChild(i).GetComponent<FriendlyGroup>();
			friendGroups[i].GroupIndex = i;
		}
	}
	private void OnEnable()
	{
		//StartCoroutine(EnemyGroupGenEvent());
	}
	

	/*IEnumerator EnemyGroupGenEvent()
	{
        float prob = 0f;//기밀탈취단 등장확률
        int GroupId = 0;

        while (gameObject.activeSelf)
		{
			yield return new WaitForSeconds(2f);
			if (EnemyGroupEvent().gameObject.activeSelf)
			{
				genGroup = EnemyGroupEvent();
				if (!genGroup.isGenerate)
				{
					genGroup.isGenerate = true;
					genGroup.GroupMemberInit(prob, GroupId, 0);		//적 그룹을 생성하지않은 곳을 찾아 생성호출
				}
				else
					yield return new WaitForEndOfFrame();
			}
		}

    }*/

    public static void ParticleGenerate(EFFECT_TYPE type, Vector3 point)
	{		
		GameObject obj = PoolManager.current.PopParticle(type);
		obj.transform.position = new Vector3(point.x, 0.5f, point.z + 0.5f);
        obj.SetActive(true);
	}

}
