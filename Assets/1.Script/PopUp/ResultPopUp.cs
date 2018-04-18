using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultPopUp : MonoBehaviour {
    public UnityEngine.UI.Button NextButton;
    public GameObject DamageReport;
    public List<GameObject> EnemyGroupList;

    public GameObject Success;
    public GameObject Fail;

    public UnityEngine.UI.Image prizePercent;  

    List<EnemyGroupResult> enemyGroupResult = new List<EnemyGroupResult>();

    int EnemyCnt;

    void SetEnemyListActive(int enemyNum) //적군 집단 수
    {
        EnemyCnt = enemyNum;

        EnemyGroupList[0].SetActive(false);
        EnemyGroupList[1].SetActive(false);
        EnemyGroupList[2].SetActive(false);

        print("enemyNum : " + enemyNum);

        EnemyGroupList[enemyNum - 1].SetActive(true);
    }

    public void InitResultPopUp()
    {
        enemyGroupResult.Clear();

        Success.SetActive(false);
        Fail.SetActive(false);

        NextButton.enabled = false;
        //필요한 데이터
        //int      EnemyCnt    : 적군 집단 수
        //string[] enemyId     : 집단 별 적군 종류 id
        //bool[]   enemyActive : 집단 별 적군 종류 별 제압 성공 여부 

        ///////임시
        int tempEnemyNum = EnemyManager.EnemyGroupMax;
        
        SetEnemyListActive(tempEnemyNum);
        ///////

        for (int i = 0; i < EnemyCnt; i++)
        {
            enemyGroupResult.Add(EnemyGroupList[EnemyCnt - 1].transform.GetChild(i).GetComponent<EnemyGroupResult>());
        }

        //////임시
        //string[] tmpEnemyId0 = { "Guard", "Guard", "QuickReactionForces", "Researcher" };
        //bool[] tmpEnemyActive0 = { true, false, false, true };
        //
        //string[] tmpEnemyId1 = { "QuickReactionForces", "BiochemistryUnit", "Researcher", "Guard" };
        //bool[] tmpEnemyActive1 = { false, false, false, false };
        //
        //string[] tmpEnemyId2 = { "Researcher", "QuickReactionForces", "Guard", "Researcher" };
        //bool[] tmpEnemyActive2 = { true, true, false, true };
        //
        //enemyGroupResult[0].InitResult(tmpEnemyId0, tmpEnemyActive0);
        //enemyGroupResult[1].InitResult(tmpEnemyId1, tmpEnemyActive1);
        //enemyGroupResult[2].InitResult(tmpEnemyId2, tmpEnemyActive2);
        ///////

        if(DayandNight.CreatedEnemy.Count < 4)
        {
            gameObject.SetActive(false);
            return;
        }

        int createdEnemyCnt =  DayandNight.CreatedEnemy.Count;
        for (int i = 0; i < EnemyCnt; i++)
        {
            string[] enemyId = new string[4];
            bool[] enemyActive = new bool[4];

            //이제 나중에 여기에서 소속집단 판별하면 될듯
            int Cnt = 0;
            for (int j = 0; j < createdEnemyCnt; j++)
            {
                if (DayandNight.CreatedEnemy[j].Group == (i + 1))
                {
                    enemyId[Cnt] = DayandNight.CreatedEnemy[j].name;
                    enemyActive[Cnt] = !DayandNight.CreatedEnemy[j].isDie;

                    if (Cnt == 3)
                        break;
                    else
                        Cnt++;
                }
            }

            if (Cnt != 3)
                print("Cnt != 3");

            enemyGroupResult[i].InitResult(enemyId, enemyActive);
        }

        StartCoroutine("PlayResult");
    }

    void SetSuccess(bool isSuccess)
    {
        Success.SetActive(isSuccess);
        Fail.SetActive(!isSuccess);
    }

    IEnumerator PlayResult()
    {
        //없앤 적군 표시
        for(int i = 0; i < enemyGroupResult.Count; i++)
        {
            enemyGroupResult[i].isDone = false;
            enemyGroupResult[i].StartCoroutine("SetEnemyActive");

            yield return new WaitUntil(enemyGroupResult[i].GetIsDone);
        }
        //없앤 적군 종류 만큼 현상금 + 달러오르고

        //print(Data_Player.Experience + "/" + Data_Player.LvExperience);
        //prizePercent.fillAmount = Data_Player.Experience / Data_Player.LvExperience;


        //제압 성공한 적군 집단 표시
        for (int i = 0; i < enemyGroupResult.Count; i++)
        {
            if (enemyGroupResult[i].isSuccess)
                enemyGroupResult[i].SetSuccess();

            yield return new WaitForSeconds(0.4f);
        }
        //제압 성공한 적군 만큼 현상금 + 달러


        yield return new WaitForSeconds(0.3f);

        //제압 실패한 적군 집단 표시
        for (int i = 0; i < enemyGroupResult.Count; i++)
        {
            if (!enemyGroupResult[i].isSuccess)
                enemyGroupResult[i].SetSuccess();

            yield return new WaitForSeconds(0.4f);
        }
        //제압 실패한 만큼 현상금 떨어짐

        yield return new WaitForSeconds(0.3f);
        //거점 성공 여부 띄움 
        SetSuccess(true); //true 아닌거 알지..?
        NextButton.enabled = true;

        //결과화면 없애고
        //일일피해 보고서 띄운다.  //이거는 클릭으로 이루어짐
        yield break;
    }
}
