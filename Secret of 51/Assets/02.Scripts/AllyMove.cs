using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AllyMove : MonoBehaviour {

    // 변수 초기화 ( f_AI_Recognize_Scope : 아군의 적군 인식 범위 )
    NavMeshAgent ally;
    Vector3 Origin_Position;
    public float f_AI_Recognize_Scope = 10f;
    
    void Awake()
    {
        // 변수 초기화 ( ally : 해당 GameObject의 NavMeshAgent형 자료, Origin_Position : 해당 GameObject의 원래 위치 )
        Origin_Position = this.transform.position;
        ally = GetComponent<NavMeshAgent>();
    }

    void Start()
    {

    }

    void Update()
    {
        MoveToEnemy(); 
    }

    void OnTriggerEnter(Collider other)
    {
        // 충돌시 Enemy 비활성화
        if (other.gameObject.CompareTag("Enemy"))
        {
            other.gameObject.SetActive(false);
        }
    }

    // 가장 가까운 적 찾기
    GameObject FindclosestEnemy()
    {
    /* 변수 초기화 ( enemys : tag가 Enemy인 GameObject를 갖고 있는 배열, closest_enemy : enemys 배열 중 가장 ally와 가까운 enemy
    distance : 가장 가까운 enemy와의 거리 ) */
        GameObject[] enemys;
        enemys = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closest_enemy = null;
        float distance = Mathf.Infinity;
        // Vector3 ally_position = this.transform.position;

        foreach (GameObject enemy in enemys)
        {
            // diff : 적의 위치와 설치된 아군 위치 간 거리벡터
            Vector3 diff = enemy.transform.position - Origin_Position;
            // curDistance : 적 위치와 설치된 아군 위치 간 거리 값
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest_enemy = enemy;
                distance = curDistance;
            }
        }
        return closest_enemy;
    }

    // 범위안의 가장 가까운 적에게 이동
    void MoveToEnemy()
    {
        // 변수 초기화( target : 가장 가까운 적의 위치 정보, f_distance : 가장 가까운 적과 아군의 거리 )
        Transform target = FindclosestEnemy().transform;
        float f_distance = Vector3.Distance(target.position, Origin_Position);

        // 범위 안에 적군 있을 시 추적
        if(f_distance <= f_AI_Recognize_Scope)
        {
            ally.SetDestination(target.transform.position);
        }

        // 범위 안에 적군 없을 시 제자리로 복귀
        else
        {
            ally.SetDestination(Origin_Position);
        }
    }
}
