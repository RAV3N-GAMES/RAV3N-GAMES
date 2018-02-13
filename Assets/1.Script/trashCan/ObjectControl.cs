using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectControl : MonoBehaviour {
    public enum Type
    {
        Wall,
        AI,
        Trap,
        Treasure
    }

    public Type ObjectType;

    public bool isCreate;

    void Awake()
    {
        if(isCreate)
            InitComponent();
    }

    void InitComponent()
    {
       
    }
}
