using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageReportPopUp : MonoBehaviour {
    const int ObjLength = 4;

    public static string[] BuildingIdx = { "OldBuilding", "NewBuilding", "FunctionalBuilding", "CoreBuilding" };
    public static string[] OurForcesIdx = { "Guard", "QuickReactionForces", "BiochemistryUnit", "Researcher" };
    public static string[] TrapIdx = { "HumanTrap", "Warp", "FlameThrowingTrap", "ObstructMovementCurrent" };
    public static string[] SecretIdx = { "UFOCore", "AlienStorageCapsule", "AlienBooldStorage", "SpaceVoiceRecordingFile" };

    public static int[] Building = new int[ObjLength];
    public static int[] OurForces = new int[ObjLength];
    public static int[] Trap = new int[ObjLength];
    public static int[] Secret = new int[ObjLength];

    public static List<int>[] BuildingLevel = { new List<int>(), new List<int>(), new List<int>(), new List<int>() };
    public static List<int>[] OurForcesLevel = { new List<int>(), new List<int>(), new List<int>(), new List<int>() };
    public static List<int>[] TrapLevel = { new List<int>(), new List<int>(), new List<int>(), new List<int>() };
    public static List<int>[] SecretLevel = { new List<int>(), new List<int>(), new List<int>(), new List<int>() };

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

    public static void PlusDamage(int type, string id, int level)
    {
        string[] damageObj;

        switch (type)
        {
            case 0: damageObj = BuildingIdx; break;
            case 2: damageObj = OurForcesIdx; break;
            case 3: damageObj = TrapIdx; break;
            case 4: damageObj = SecretIdx; break;
            default: return;
        }

        for(int i = 0; i < ObjLength; i++)
        {
            if(damageObj[i].Equals(id))
            {
                switch (type)
                {
                    case 0: Building[i]++; BuildingLevel[i].Add(level); break;
                    case 2: OurForces[i]++; OurForcesLevel[i].Add(level); break;
                    case 3: Trap[i]++; TrapLevel[i].Add(level); break;
                    case 4: Secret[i]++; SecretLevel[i].Add(level); break;
                    default: return;
                }
                return;
            }
        }
    }

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
    }

    void SetActive()
    {
        OkSprite.SetActive(false);
        gameObject.SetActive(false);
    }

    public static void InitDamageIdx()
    {
        for (int i = 0; i < ObjLength; i++)
        {
            Building[i] = 0;
            OurForces[i] = 0;
            Trap[i] = 0;
            Secret[i] = 0;

            BuildingLevel[i].Clear();
            OurForcesLevel[i].Clear();
            TrapLevel[i].Clear();
            SecretLevel[i].Clear();
        }
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

        print("maxNum : " + maxNum);

        StartCoroutine("PlayDamageReport");
    }

    IEnumerator PlayDamageReport()
    {
        //버튼 클릭 막고
        for(int loopCnt = 0; loopCnt < (maxNum + 1); loopCnt++)
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
        //클릭 풀고
        InitDamageIdx();
        yield break;
    }
}
