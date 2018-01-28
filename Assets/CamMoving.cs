using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamMoving : MonoBehaviour {
	public Transform cam;
	public Camera cam_zoom;
	public float Speed;
	public Vector2 nowPos, prePos, movePos;

	// Use this for initialization
	void Start () {
		cam_zoom.orthographic = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown (0)) {
			prePos = Input.mousePosition;
		}else if(Input.GetMouseButton(0)){
			nowPos = Input.mousePosition;
			movePos = (prePos - nowPos) * Speed;
			cam.transform.Translate (movePos);
			prePos = Input.mousePosition;
		}

		cam_zoom.orthographicSize += Input.GetAxis ("Mouse ScrollWheel") * 50 * -1;


	}
}
