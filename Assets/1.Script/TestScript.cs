using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestScript : MonoBehaviour {
    NavMeshAgent nav;
    public Transform T;
    public Transform T2;
    public Transform T3;
    public string Choosed;

	// Use this for initialization
	void Start () {
        nav = GetComponent<NavMeshAgent>();
        NavMeshPath p1=new NavMeshPath(), p2=new NavMeshPath(), p3=new NavMeshPath();
        nav.CalculatePath(T.position, p1);
        nav.CalculatePath(T2.position, p2);
        nav.CalculatePath(T3.position, p3);
        Choosed = "T1";

        nav.SetPath(p1);
        //nav.SetDestination(T.transform.position);
        if (nav.pathStatus == NavMeshPathStatus.PathPartial || nav.pathStatus == NavMeshPathStatus.PathInvalid)
        {
            nav.SetPath(p2);
            if (nav.pathStatus == NavMeshPathStatus.PathPartial || nav.pathStatus == NavMeshPathStatus.PathInvalid)
            {
                Debug.Log("P3 Accepted");
                nav.SetPath(p3);
            }
            else {
                Debug.Log("p2 Accepted");
            }
        }
        else
            Debug.Log("P1 accepted");

        
    }
	
	// Update is called once per frame
	void Update () {

    }


}
