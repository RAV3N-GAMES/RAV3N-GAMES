using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckRoomIndex : MonoBehaviour {
    public static int roomidx;

    // Use this for initialization
    void Start () {
        roomidx = -1;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Tile"))
        {
            try
            {
                roomidx = int.Parse(col.transform.parent.transform.parent.transform.parent.name);
                Debug.Log("Roomidx in Trigger: " + roomidx);
            }
            catch
            {
                Debug.Log("Col : " + this.name);
            }
        }
    }
}
