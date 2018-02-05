using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMove : MonoBehaviour {

    public GameObject dest;
    NavMeshAgent enemy;

    void Awake()
    {
        enemy = GetComponent<NavMeshAgent>();
        dest = GameObject.FindGameObjectWithTag("Base");
    }
    // Use this for initialization

    void Start ()
    {
	
	}

    // Update is called once per frame
    void Update()
    {
           enemy.SetDestination(dest.transform.position);
	}

}
