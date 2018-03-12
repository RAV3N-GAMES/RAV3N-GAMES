using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum DIRECTION
{
	RIGHT,
	LEFT,
}

public class GameManager : MonoBehaviour {
	public static GameManager current;
	public static float GameProgressTimer;

	public RoomManager roomManager;

	public delegate EnemyGroup GetEnemyGroup();
	public delegate FriendlyGroup GetFriendGroup();

	public static event GetEnemyGroup EnemyGroupEvent;
	public static event GetFriendGroup FriendlyGroupEvent;

	public Transform GroupParent;
	public Transform CommandPost;

	public EnemyGroup[]	enemyGroups;
	public FriendlyGroup[] friendGroups;

	public GameObject UiSelectObj;

	public CubeObject getRayTrans;

	private EnemyGroup genGroup = null;
	private WaitForSeconds waitGenDelay;

	private LayerMask tileMask;
	private bool isFloorClick;

	
	public int GetRoomIndex
	{
		get { return roomManager.CenterRoomIdx; }
	}
	private void Awake()
	{
		if (current == null)
			current = this;
		else
			Destroy(gameObject);

		if (GameProgressTimer <= 0)
			GameProgressTimer = 2;

		waitGenDelay = new WaitForSeconds(GameProgressTimer);
		tileMask = LayerMask.GetMask("Tile");
		GroupInit();
	}
	private void Update()
	{
		RayEvent();
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
		StartCoroutine(EnemyGroupGenEvent());
	}
	

	IEnumerator EnemyGroupGenEvent()
	{
		while (gameObject.activeSelf)
		{
			yield return waitGenDelay;
			if (EnemyGroupEvent().gameObject.activeSelf)
			{
				genGroup = EnemyGroupEvent();
				if (!genGroup.isGenerate)
				{
					genGroup.isGenerate = true;
					genGroup.GroupMemberInit();		//적 그룹을 생성하지않은 곳을 찾아 생성호출
				}
				else
					yield return new WaitForEndOfFrame();
			}
		}
	}

	public static void ParticleGenerate(EFFECT_TYPE type, Vector3 point)
	{
		GameObject obj = PoolManager.current.PopParticle(type);
		obj.transform.position = new Vector3(point.x, 5f, point.z + 0.5f);
	}

}
