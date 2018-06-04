using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//
public class EnemyMafia : Enemy {
    
    private void Start()
	{
        Clips = Resources.LoadAll<AudioClip>(path + "Enemy/Mafia") as AudioClip[];
        DieClip = Resources.LoadAll<AudioClip>(path + "Die/Man") as AudioClip[];
        effectType = EFFECT_TYPE.Hit;
		attackDelay = new WaitForSeconds(1.2f);
        diaglogue[0] = "전 돈 많습니다, 현상금은 관심 없어요";
        diaglogue[1] = "혹시 사채 필요하십니까";
        diaglogue[2] = "돈으로 살 수 없다 길래 훔치러 왔습니다";
        diaglogue[3] = "고용한 내 경호원들 다 어디 간 거야";
    }
	public override void EnemyInit()
	{
        scollider.radius =  3.0f;
        Hp = EnemyManager.Tbl_EnemySetup[Data_Player.Fame + 18].HP;
        Attack = EnemyManager.Tbl_EnemySetup[Data_Player.Fame + 18].Attack;
		MaxHp = Hp;
        isHealer = false;
		base.EnemyInit();
        enemyAI.stoppingDistance = (float)EnemyManager.Tbl_EnemySetup[Data_Player.Fame + 18].AttackRange;
	}
}
