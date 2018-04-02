using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {
	public static UIManager current;
	public GameObject UIBarPrefab;
	public Transform UIBarParent;

	// Use this for initialization
	void Awake () {
		if (current == null)
			current = this;
		else
			Destroy(gameObject);
	}
}
