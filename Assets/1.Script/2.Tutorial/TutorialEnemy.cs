using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialEnemy : MonoBehaviour {
    public GameObject EnemyPref;
    public Enemy E;//Enemy 스크립트랑 navmesh만 붙은 전용 Enemy 생성. 체력 1
    public Friendly F;
    public DayandNight Curtain;
    public TutorialTile Tt;
    public bool Created = false;
    void Update() {
        if (E.isDie) {
            Curtain.changeState();
        }
        if (DayandNight.isDay && !Created )
        {
            StartCoroutine(CreateEnemy());
            Created = true;
        }
    }
    
    public IEnumerator CreateEnemy() {
        EnemyPref.transform.position = Tt.transform.position + Vector3.right * 10;
        E.Hp = 1;
        E.isDie = false;
        E.Attack = 0;
        E.GroupConductor = GameManager.current.enemyGroups[1];
        E.myCluster= EnemyClusterManager.clusterList[0];
        GameObject g = GameObject.Find("RecognizeRange").transform.parent.gameObject;
        Debug.Log("G: " + g);
        Debug.Log(g.transform.Find("Friendly_Guard"));
        Debug.Log(g.transform.Find("Friendly_Guard").gameObject.activeSelf);
        yield return new WaitUntil( () => g.transform.Find("Friendly_Guard").gameObject.activeSelf);
        F=g.GetComponentInChildren<Friendly>();//가드 외 다른거 할 경우 변경
        Debug.Log("F: " + F);
        EnemyPref.SetActive(true);
        TutorialEnemyAction();
    }

    public void TutorialEnemyAction() {
        E.dest = E.SetYZero(F.NavObj.transform);
        E.enemyAI.SetDestination(E.dest);
        E.currentState = EnemyState.Walk;
    }
}
