using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomBoundary : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnTriggerEnter(Collider col) {
        if (col.CompareTag("EnemyBody")) { 
            EnemyGroup EG = GetComponentInParent<EnemyGroup>();
            EG.ChildTriggered(col);
        }
    }
}
