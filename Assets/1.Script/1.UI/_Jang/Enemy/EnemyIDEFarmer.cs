using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIDEFarmer : Enemy {

    private void Start()
    {
        Clips = Resources.LoadAll<AudioClip>(path + "Enemy/IDE") as AudioClip[];
        DieClip = Resources.LoadAll<AudioClip>(path + "Die/Man") as AudioClip[];
        effectType = EFFECT_TYPE.Approach;
        attackDelay = new WaitForSeconds(1f);
        diaglogue[0] = "난 돈이필요해.. 죽고싶지않으면 비켜";
        diaglogue[1] = "!@#$%&";
        diaglogue[2] = "외계인을 허수아비로 쓸거야 죽고싶지 않으면 비켜";
        diaglogue[3] = "너 내 농장에서 허수아비해라 아니면 죽던가";
    }
    public override void EnemyInit()
    {
        scollider.radius = 3.0f;
        Hp = EnemyManager.Tbl_EnemySetup[Data_Player.Fame + 62].HP;
        Attack = EnemyManager.Tbl_EnemySetup[Data_Player.Fame + 62].Attack;
        MaxHp = Hp;
        isHealer = false;
        base.EnemyInit();
        enemyAI.stoppingDistance = (float)EnemyManager.Tbl_EnemySetup[Data_Player.Fame + 62].AttackRange;
        enemyAI.speed = 1.0f;
    }
}
