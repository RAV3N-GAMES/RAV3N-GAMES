using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour {
    public int Level;
    public int Type;
    public string id;
    public int HP;
    public int MaxHP;
    public int Price;
    public int UpgraeCost;
    public int RepairCost;
    public int ActiveCost;
    public ObjectInfo info;
    public DisplayObject displayObject;
    protected virtual void WallInit() {
        Debug.Log("Virtual WallIniit");
        info = GetComponentInParent<ObjectInfo>();
        displayObject = GetComponentInParent<DisplayObject>();
        Type = info.type;
        id = info.id;
        Level = info.level;
        HP = info.presentHP;
        //그 외 정보는 각각 Builidng 스크립트에서 override 해서 사용
    }

    public virtual void WallSync() {
        WallInit();
    }
    private void WallSyncInfo() {//변동사항 있을 경우 object info에 이를 동기화
        info.presentHP = HP;
        info.level = Level;
        info.id = id;
        info.totalHP = MaxHP;
        info.type = Type;
        //더 추가할거 있으면 추후에 추가.
    }

    public void GetDamaged(int Damage) {
        HP -= Damage;
        WallSyncInfo();
    }

    public bool IsDestroyed() {
        if (HP < 0)
        {
            return true;//벽 깨지면 true 반환
        }
        return false;
    }

    public void DestoryWall() {
        Destroy(transform.parent.gameObject);
        displayObject.DestroyObj(true);
    }
    void Start() {
        WallInit();
    }


    // Update is called once per frame
    void Update () {
		
	}
}
