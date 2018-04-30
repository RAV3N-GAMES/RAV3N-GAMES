using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeObject : MonoBehaviour {
    
	public Transform CubeTrans;

	private void Start()
	{
		CubeTrans = transform.parent;
	}
}
