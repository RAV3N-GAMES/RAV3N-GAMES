using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySheriff : Enemy {
    private void Start()
	{
        Clips = Resources.LoadAll<AudioClip>(path + "Enemy/Sheriff") as AudioClip[];
        DieClip = Resources.LoadAll<AudioClip>(path + "Die/Man") as AudioClip[];
        effectType = EFFECT_TYPE.Hit;
		attackDelay = new WaitForSeconds(2f);
        diaglogue[0] = "현상금만 받으면 이 지긋지긋한 경찰 생활도 끝이거든";
        diaglogue[1] = "현상금 받을 때 까지 휴가입니다";
        diaglogue[2] = "경찰입니다, 물건 한 개만 찾아서 조용히 돌아가겠습니다";
        diaglogue[3] = "저 진짜 경찰 맞습니다";
    }
	public override void EnemyInit()
	{
        scollider.radius = 3.0f;
        Hp = EnemyManager.Tbl_EnemySetup[Data_Player.Fame + 84].HP;
        Attack = EnemyManager.Tbl_EnemySetup[Data_Player.Fame + 84].Attack;
        MaxHp = Hp;
        isHealer = false;
        base.EnemyInit();
        enemyAI.stoppingDistance = (float)EnemyManager.Tbl_EnemySetup[Data_Player.Fame + 84].AttackRange;
        Stoppingdistance = enemyAI.stoppingDistance;
    }
}
