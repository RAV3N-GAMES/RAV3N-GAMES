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
        Debug.Log("hello");
        StartCoroutine(ienu1());
        Debug.Log("Hello2");
        StartCoroutine(ienu2());
        Debug.Log("Hello3");
    }
	
	// Update is called once per frame
	void Update () {

    }

    IEnumerator ienu1() {
        Debug.Log("hi1");
        yield return true;
        Debug.Log("hi2");
    }

    IEnumerator ienu2() {
        Debug.Log("hi3");
        yield return true;
        Debug.Log("hi4");
    }
}
