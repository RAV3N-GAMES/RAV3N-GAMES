using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateObject : MonoBehaviour {
    [HideInInspector]
    public string id;
    bool isCreate;

    void Awake()
    {
        id = "";
    }

	public void MouseDown()
    {
        isCreate = true;
    }

    public void MouseUp()
    {
        isCreate = false;
        id = "";
    }

    public void MouseExit()
    {
        if (isCreate && id != "")
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 0;
            GameObject newObj = Instantiate(Resources.Load("Object/" + id) as GameObject);
            newObj.name = id;
            newObj.transform.position = pos;
        }
    }
}
