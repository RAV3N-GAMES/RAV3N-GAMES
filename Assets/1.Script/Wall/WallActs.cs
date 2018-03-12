using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//각 벽에 대한 설명(키값: Explain) 또한 Json에 저장. 나중에 하세요

public class WallActs : MonoBehaviour {
    ObjectInfo thisObject;

	// Use this for initialization
	void Awake () {
        thisObject = GetComponent<ObjectInfo>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    
}
