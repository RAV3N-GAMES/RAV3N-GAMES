using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTile : MonoBehaviour {
    public List<GameObject> lastColList;
    public string objName;
    public int Cnt;

    void Awake()
    {
        lastColList = new List<GameObject>();
    }

    public void OffObjectMove()
    {
        lastColList[0].GetComponent<ObjectMove>().enabled = false;
    }

    public bool isSuccess()
    {
        if (lastColList.Count == Cnt)
        {
            if (objName.Equals("Guard"))
            {
                print("Minus HP");
                lastColList[0].GetComponent<ObjectInfo>().SetHP(-1);
            }
            return true;
        }
        return false;
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Tile")
        {
            if (col.gameObject.transform.parent.parent.name == objName)
                lastColList.Add(col.transform.parent.parent.gameObject);
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "Tile")
        {
            if (col.gameObject.transform.parent.parent.name == objName)
                lastColList.Remove(col.transform.parent.parent.gameObject);
        }
    }
}
