using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapAct : MonoBehaviour {
    Animator Ani_Trap;
    public static bool isCatch;

	// Use this for initialization
	void Start () {
        Ani_Trap = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag.Equals("Enemy"))
        {
            //물린 적에게 데미지(필요하면?)
            
        }
    }
}
