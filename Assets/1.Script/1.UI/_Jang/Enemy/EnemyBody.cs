using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBody : MonoBehaviour {
    public void OnTriggerEnter(Collider col) {
        if (col.CompareTag("Tile"))
        {
            if (col.GetComponentInParent<EnemyGroup>().gameObject.activeSelf) {
                Debug.Log(transform.parent.name+" Triggered : " + col.GetComponentInParent<EnemyGroup>().GroupIndex);
                GetComponentInParent<Enemy>().PresentRoomidx = col.GetComponentInParent<EnemyGroup>().GroupIndex;
            }
        }
    }
	
}
