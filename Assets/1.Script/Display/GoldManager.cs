using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoldManager : MonoBehaviour {

    public Text goldText;
    private int gold = 0; // 골드 초기화

    public void EarnGold(int num) // 골드 추가 함수
    {
        gold += num; // 골드 더하기
        goldText.text = gold + "G"; // 텍스트에 반영
    }

    void Start()
    {

    }

    void Update()
    {

    }
}
