using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotArray : MonoBehaviour {
    public Transform Box;
    GameObject lastCol;

    public void Array()
    {
        if (lastCol != null)
        {
            Vector3 pos = transform.position - lastCol.transform.position;
            Box.transform.position += new Vector3(0, pos.y, 0);
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Slot")
        {
            lastCol = col.gameObject;
        }
    }
}
