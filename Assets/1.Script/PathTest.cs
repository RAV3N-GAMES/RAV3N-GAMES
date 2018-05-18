using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PathTest : MonoBehaviour {
    public NavMeshAgent nav;

	// Use this for initialization
	void Start () {
        nav.SetDestination(new Vector3(0, 0, 0));
        Debug.Log("path: " + nav.path);
	}
	
	// Update is called once per frame
	void Update () {
        Debug.Log("path update: " + nav.path);
	}
}
