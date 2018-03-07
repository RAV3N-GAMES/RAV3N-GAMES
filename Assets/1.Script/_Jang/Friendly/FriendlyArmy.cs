using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//2타일 거리 전방 60도 범위
//직경 5타일 범위에 공격력의 150% 데미지(소수점 아래 올림)
public class FriendlyArmy : Friendly {

	//백터 내적 변수//
	private float dotAngle;
	private float innerAngle;
	private Vector3 standard;
	private Vector3 targetDir;
	//-------------//

	Enemy triggerEnemy;

	private void Start()
	{
		Hp = 120;
		AttackDamage = 22;
		FriendType = FRIEND_TYPE.Army;
		effectType = EFFECT_TYPE.Hit;
		StopDistance = 2;
		attackDelay = new WaitForSeconds(1.2f);
	}

	public override Friendly GetFriend()
	{
		return this;
	}
	
	
	// 전방 60도 안에 있는지 체크 > 변경 할 것) -> 제대로 안돌아감
	private bool InnerSee(Transform targetNav)
	{
		standard = new Vector3(-NavObj.forward.x, 0, -NavObj.forward.z);
		targetDir.x = targetNav.position.x - NavObj.position.x;
		targetDir.y = 0;
		targetDir.z = targetNav.position.z - NavObj.position.z;
		targetDir.Normalize();

		dotAngle = Vector3.Dot(standard, targetDir);
		innerAngle = Mathf.Acos(dotAngle) * Mathf.Rad2Deg;

		if (innerAngle >= 60 && innerAngle <= 120)
		{
			return true;
		}
		else
			return false;

	}

	//현재 캐릭터의 정보를 가져오기 위한
	private void OnTriggerStay(Collider other)
	{
		//적을 인식하면 적 중 
		//아군이 적을 인식하면 인식한 순으로
		//적군이 아군을 인식하면 아군 중 가장 강한 or 힐러 공격 
		if(other.CompareTag("Enemy"))
		{
			if (targetEnemy == null)
			{
				triggerEnemy = other.GetComponent<Enemy>();
				// 범위 안에 들어왔고 시야범위안에 있으면
				//if (InnerSee(triggerEnemy.NavObj) == true)
				//{
				targetEnemy = triggerEnemy;
				GroupConductor.GroupCall(targetEnemy);
				//}bat
			}
		}
	}

}
