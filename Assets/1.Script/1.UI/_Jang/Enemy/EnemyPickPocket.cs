using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPickPocket : Enemy {
    public Trap targetTrap;

    // Use this for initialization
    private void Start()
    {
        Clips = Resources.LoadAll<AudioClip>(path + "Enemy/PickPocket") as AudioClip[];
        DieClip = Resources.LoadAll<AudioClip>(path + "Die/Man") as AudioClip[];
        effectType = EFFECT_TYPE.Approach;
        attackDelay = new WaitForSeconds(1f);
        targetTrap = null;
    }
    public override void EnemyInit()
    {
        scollider.radius = (float)EnemyManager.Tbl_EnemySetup[Data_Player.Fame + 40].AttackRange * 6;
        Hp = EnemyManager.Tbl_EnemySetup[Data_Player.Fame + 40].HP;
        Attack = EnemyManager.Tbl_EnemySetup[Data_Player.Fame + 40].Attack;
        MaxHp = Hp;
        isHealer = false;
        base.EnemyInit();
        enemyAI.stoppingDistance = scollider.radius;
        enemyAI.speed = 1.0f;
    }
}
