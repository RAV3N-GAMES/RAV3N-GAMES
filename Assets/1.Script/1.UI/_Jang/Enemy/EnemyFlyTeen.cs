using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFlyTeen : Enemy
{
    private void Start()
	{
        Clips = Resources.LoadAll<AudioClip>(path+"Enemy/FlyTeen") as AudioClip[];
        DieClip= Resources.LoadAll<AudioClip>(path + "Die/Man") as AudioClip[];
        effectType = EFFECT_TYPE.Approach;
		attackDelay = new WaitForSeconds(1f);
        diaglogue[0] = "현상금을 타서 오토바이를 하나 장만할 겁니다";
        diaglogue[1] = "이 칼 장난감 아닌데요";
        diaglogue[2] = "UFO를 훔쳐서 타고다닐거야!";
        diaglogue[3] = "내가 외계인을 봤어요!";
    }
	public override void EnemyInit()
	{
        scollider.radius = (float)EnemyManager.Tbl_EnemySetup[Data_Player.Fame-4].AttackRange*6;
        Hp = EnemyManager.Tbl_EnemySetup[Data_Player.Fame-4].HP;
		Attack = EnemyManager.Tbl_EnemySetup[Data_Player.Fame-4].Attack;
        MaxHp = Hp;
        isHealer = false;
		base.EnemyInit();
        enemyAI.stoppingDistance = scollider.radius;
    }
}
