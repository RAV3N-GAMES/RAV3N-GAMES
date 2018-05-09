using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySinger : Enemy {
    WaitForSeconds healDelay = new WaitForSeconds(2f);

	private void Start()
	{
        Clips = Resources.LoadAll<AudioClip>(path + "Enemy/Singer") as AudioClip[];
        DieClip = Resources.LoadAll<AudioClip>(path + "Die/Woman") as AudioClip[];
        effectType = EFFECT_TYPE.Heal;
		isDie = false;
	}
	public override void EnemyInit()
	{
        scollider.radius = (float)EnemyManager.Tbl_EnemySetup[Data_Player.Fame + 106].AttackRange * 2;
        Hp = EnemyManager.Tbl_EnemySetup[Data_Player.Fame + 106].HP;
        Attack = EnemyManager.Tbl_EnemySetup[Data_Player.Fame + 106].Attack;
        MaxHp = Hp;
        isHealer = true;
        base.EnemyInit();
        enemyAI.stoppingDistance = scollider.radius;
    }

    protected override IEnumerator GiveHeal()
    {
        if (healTarget == null)
            yield break;

        healTarget.Health(-Attack);
        GameManager.ParticleGenerate(effectType,
            healTarget.NavObj.position);

        yield return healDelay;
        isHeal = false;
    } 
}
