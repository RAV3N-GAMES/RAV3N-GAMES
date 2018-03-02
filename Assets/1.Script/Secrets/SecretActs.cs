using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretActs : MonoBehaviour {
    public List<Data> SecretData=new List<Data>();
    private int Lv;//각 기밀들의 레벨!!!! 플레이어 레벨(fame)아님!
    private int Price;
    private float Chance;//기밀이 목적인 적 출현 확률

    // Use this for initialization
    void Awake () {
        //현재 맵에 있는 Secret의 수를 1 늘린다. 최대치면 해당 gameObject를 삭제한다.
        if (++SecretManager.SecretCount >= SecretManager.GetSecretLimit())
            Destroy(this.gameObject);
        SecretData = SecretManager.Tbl;
        Chance = 0;
    }

    // Update is called once per frame
    void Update () {
		
	}

    public float getChance() { return Chance; }
    public int getPrice() { return Price; }
    public int getLv() { return Lv; }
    public void setChance(float chance) { Chance = chance; }
    public void setPrice(int price) { Price = price; }
    public void setLv(int lv) { Lv = lv; }

    public void upLv(int operand) {
        Lv += operand;
        Debug.Log("Level up! By"+operand+" Now " + Lv + "lv");
    }
}
