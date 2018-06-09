using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanTrap : Trap {
    // Use this for initialization
	void Start () {
        TrapManager.TrapCount[0]++;
        TrapAni = GetComponent<Animator>();
        CoolTime = TrapManager.Tbl_TrapSetup[Data_Player.Fame-4].CoolTime;
        EffectContinuousTime = TrapManager.Tbl_TrapSetup[Data_Player.Fame - 4].EffectContinuousTime;
        isCool = false;
    }

    // Update is called once per frame
    void Update () {
		
	}
    
    public override void Acts(Collider col) {
        //Trap 클래스의 on collision 함수에서 EnemyBody 태그를 가진 오브젝트가 콜라이더에 닿으면 실행. 
        //쿨타임 적용
        StartCoroutine(CatchStatus(EffectContinuousTime, col));
    }

    IEnumerator CatchStatus(double EffectContinuousTime, Collider col) {
        isCool = true;
        TrapAni.Play("Trap", -1, 0.5f);//중간(잡는 모션) 실행
        TrapAni.speed = 0;//바로 정지
        Audio.Play();
        CatchEnemy(col);
        yield return new WaitForSeconds((float)EffectContinuousTime);
        TrapAni.speed = 1;
        FreeEnemy(col);
        yield return new WaitForSeconds((float)CoolTime);
        isCool = false;
    }
    
    void CatchEnemy(Collider col) {//포획 대상 정지
        Enemy e = col.GetComponentInParent<Enemy>();
        e.Movable = false;
    }
    void FreeEnemy(Collider col) {//포획 대상 풀어줌
        Enemy e = col.GetComponentInParent<Enemy>();
        e.Movable = true;
    }

}
