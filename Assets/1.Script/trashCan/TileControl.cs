using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileControl : MonoBehaviour {
    public bool isUsing { get; private set; }
    GameObject Obj;

    void Start()
    {
        try
        {
            Obj = transform.GetChild(0).gameObject;
        }
        catch { Obj = null; }
    }

    public bool CreateObject(GameObject _Obj)
    {
        if (!isUsing)
        {
            isUsing = true;
            Obj = _Obj;
            return true;
        }
        else
            return false;
    }

    public void DeleteObject()
    {
        isUsing = false;
        Obj = null;
    }
}
