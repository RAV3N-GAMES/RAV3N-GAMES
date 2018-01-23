using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour {
	public Transform Target;
	public float dist = 10f;
	public float height = 3f;
	public float dampTrace = 20f;

	void LateUpdate(){
		transform.position = Vector3.Lerp (transform.position,
			Target.position - (Target.forward * dist) + (Vector3.up * height),
			Time.deltaTime * dampTrace);
		transform.LookAt (Target.position);
	}
}
