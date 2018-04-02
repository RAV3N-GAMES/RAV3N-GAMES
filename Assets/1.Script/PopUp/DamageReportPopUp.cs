using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageReportPopUp : MonoBehaviour {
    const int ObjLength = 4;

    int[] Building = new int[ObjLength]; 
    int[] OurForces = new int[ObjLength];
    int[] Trap = new int[ObjLength];
    int[] Secret = new int[ObjLength];

    public Transform BuildingParent;
    public Transform OurForcesParent;
    public Transform TrapParent;
    public Transform SecretParent;

    public GameObject OkSprite;

    List<Text> TextBuilding = new List<Text>();
    List<Text> TextOurForces = new List<Text>();
    List<Text> TextTrap = new List<Text>();
    List<Text> TextSecret = new List<Text>();

    int maxNum;

    void InitTextList(Transform parent, List<Text> textList)
    {
        for(int i = 0; i < parent.childCount; i++)
        {
            textList.Add(parent.GetChild(i).gameObject.GetComponent<Text>());
        }
    }

    void Awake()
    {
        InitTextList(BuildingParent, TextBuilding);
        InitTextList(OurForcesParent, TextOurForces);
        InitTextList(TrapParent, TextTrap);
        InitTextList(SecretParent, TextSecret);

        //임시
        for (int i = 0; i < ObjLength; i++)
        {
            Building[i] = i;
            OurForces[i] = i % 2;
            Trap[i] = i + 1;
            Secret[i] = 1;
        }
    }

    void SetActive()
    {
        OkSprite.SetActive(false);
        gameObject.SetActive(false);
    }

    public void ClickOk()
    {
        Invoke("SetActive", 2f);
    }

    public void InitDamageReport()
    {
        maxNum = 0;

        for (int i = 0; i < ObjLength; i++)
        {
            if (Building[i] > maxNum)
                maxNum = Building[i];
            if (OurForces[i] > maxNum)
                maxNum = OurForces[i];
            if (Trap[i] > maxNum)
                maxNum = Trap[i];
            if (Secret[i] > maxNum)
                maxNum = Secret[i];

            TextBuilding[i].text = "X0";
            TextOurForces[i].text = "X0";
            TextTrap[i].text = "X0";
            TextSecret[i].text = "X0";
        }

        StartCoroutine("PlayDamageReport");
    }

    IEnumerator PlayDamageReport()
    {
        for(int loopCnt = 0; loopCnt < maxNum; loopCnt++)
        {
            for(int objCnt = 0; objCnt < ObjLength; objCnt++)
            {
                if (Building[objCnt] >= loopCnt)
                    TextBuilding[objCnt].text = "X" + loopCnt.ToString();
                if (OurForces[objCnt] >= loopCnt)
                    TextOurForces[objCnt].text = "X" + loopCnt.ToString();
                if (Trap[objCnt] >= loopCnt)
                    TextTrap[objCnt].text = "X" + loopCnt.ToString();
                if (Secret[objCnt] >= loopCnt)
                    TextSecret[objCnt].text = "X" + loopCnt.ToString();
            }

            yield return new WaitForSeconds(0.5f);
        }

        yield break;
    }
}
