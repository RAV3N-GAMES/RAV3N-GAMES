using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//각 벽에 대한 설명(키값: Explain) 또한 Json에 저장. 나중에 하세요

public class WallActs : MonoBehaviour {
    ObjectInfo thisObject;

    // Use this for initialization
    void Awake() {
        thisObject = GetComponent<ObjectInfo>();
    }

    // Update is called once per frame
    void Update() {

    }
    public void Hit(int Damage)//Enemy에서 Wall 타격시 호출
    {
        thisObject.presentHP -= Damage;
        if (thisObject.presentHP <= 0)
            DestroyWall();
    }
    public void DestroyWall() {
        DisplayObject thisDisplay = GetComponent<DisplayObject>();
        thisDisplay.DestroyObj();
        Destroy(this.gameObject);
    }
}
