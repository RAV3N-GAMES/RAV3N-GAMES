using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warp_Exit : Trap {
    public GameObject Effect;
    Warp_Exit() { }

    Warp_Exit(Vector3 pos) {
        transform.position = pos;
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
