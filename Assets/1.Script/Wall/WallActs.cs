using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//각 벽에 대한 설명(키값: Explain) 또한 Json에 저장. 나중에 하세요

public class WallActs : MonoBehaviour {
    ObjectInfo thisObject;

    // Use this for initialization
    void Awake() {
        thisObject = GetComponent<ObjectInfo>();
        initBuilding(thisObject);
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

    void initBuilding(ObjectInfo info) {
        switch (info.id)
        {
            case "FunctionalBuilding":
                //6x6반경에 있는 TrapActs에 bool type isSafe 생성 -> 이를 true로 변경
                //6x6 반경은 콜라이더로 해결.

                break;
            case "CoreBuilding":
                //올라온 원거리캐릭사거리 증가
                //원거리캐릭 인지범위 늘리는걸로 해결

                break;
        }
    }
}
