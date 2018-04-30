using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultPopUp : MonoBehaviour {
    public Button NextButton;
    public GameObject DamageReport;
    public List<GameObject> EnemyGroupList;

    public GameObject Success;
    public GameObject Fail;

    public Image prizePercent;
    public Text Fame;
    public Text Coin;
    public Text Exp;

    List<EnemyGroupResult> enemyGroupResult = new List<EnemyGroupResult>();

    int EnemyCnt;

    void SetEnemyListActive(int enemyNum) //적군 집단 수
    {
        EnemyCnt = enemyNum;

        EnemyGroupList[0].SetActive(false);
        EnemyGroupList[1].SetActive(false);
        EnemyGroupList[2].SetActive(false);

        EnemyGroupList[enemyNum - 1].SetActive(true);
    }

    public void InitResultPopUp()
    {
        enemyGroupResult.Clear();

        Success.SetActive(false);
        Fail.SetActive(false);

        NextButton.enabled = false;

        Exp.text = Data_Player.Experience.ToString();
        Coin.text = Data_Player.Gold.ToString();

        //필요한 데이터
        //int      EnemyCnt    : 적군 집단 수
        //string[] enemyId     : 집단 별 적군 종류 id
        //bool[]   enemyActive : 집단 별 적군 종류 별 제압 성공 여부 
        
        int tempEnemyNum = EnemyManager.EnemyGroupMax;
        SetEnemyListActive(tempEnemyNum);

        for (int i = 0; i < EnemyCnt; i++)
        {
            enemyGroupResult.Add(EnemyGroupList[EnemyCnt - 1].transform.GetChild(i).GetComponent<EnemyGroupResult>());
        }

        if(DayandNight.CreatedEnemy.Count < 4)
        {
            gameObject.SetActive(false);
            return;
        }

        int createdEnemyCnt =  DayandNight.CreatedEnemy.Count;
        int[] GroupId = new int[EnemyCnt];
        int GroupCnt = 0;

        for(int i = 0; i < DayandNight.CreatedEnemy.Count; i++)
        {
            for (int j = 0; j < GroupCnt + 1; j++)
            {
                if (GroupId[j] != DayandNight.CreatedEnemy[i].Group)
                {
                    GroupId[GroupCnt++] = DayandNight.CreatedEnemy[i].Group;
                    break;
                }
            }

            if (GroupCnt == EnemyCnt)
                break;
        }


        for (int i = 0; i < EnemyCnt; i++)
        {
            string[] enemyId = new string[4];
            bool[] enemyActive = new bool[4];
            
            int Cnt = 0;
            for (int j = 0; j < createdEnemyCnt; j++)
            {
                if (DayandNight.CreatedEnemy[j].Group == GroupId[i])
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

        yield return new WaitForSeconds(0.3f);

        //없앤 적군 종류 만큼 현상금 + 달러오르고
        int firstExp = 100; 
        int secondExp = 0;

        int coin = 1000;
        
        if (firstExp + Data_Player.Experience >= Data_Player.LvExperience)
            secondExp = firstExp - (Data_Player.LvExperience - Data_Player.Experience);

        for (int i = 0; i < 100; i++)
        {
            Data_Player.addExperience(firstExp / 100);
            prizePercent.fillAmount = (float)Data_Player.Experience / (float)Data_Player.LvExperience;

            Data_Player.addGold(coin / 100);
            Coin.text = Data_Player.Gold.ToString();
            Exp.text = Data_Player.Experience.ToString();

            yield return new WaitForSeconds(0.02f);
        }

        if(secondExp != 0)
        {
            yield return new WaitForSeconds(0.3f);
            /////////////////////////만약에 레벨 더 올라가는 경우를 위해 예외처리 필요////////////////////////
            //여기에서 레벨 텍스트 올리고 올리고
            for (int i = 0; i < 100; i++)
            {
                Data_Player.addExperience(secondExp / 100);
                prizePercent.fillAmount = (float)Data_Player.Experience / (float)Data_Player.LvExperience;

                Exp.text = Data_Player.Experience.ToString();

                yield return new WaitForSeconds(0.02f);
            }
        }
        yield return new WaitForSeconds(0.3f);

        //제압 성공한 적군 집단 표시
        for (int i = 0; i < enemyGroupResult.Count; i++)
        {
            if (enemyGroupResult[i].isSuccess)
            {
                enemyGroupResult[i].SetSuccess();
                yield return new WaitForSeconds(0.4f);
            }
        }
        yield return new WaitForSeconds(0.3f);

        //제압 성공한 적군 만큼 현상금 + 달러
        firstExp = 100;
        secondExp = 0;

        coin = 1000;
        
        if (firstExp + Data_Player.Experience >= Data_Player.LvExperience)
            secondExp = firstExp - (Data_Player.LvExperience - Data_Player.Experience);

        for (int i = 0; i < 100; i++)
        {
            Data_Player.addExperience(firstExp / 100);
            prizePercent.fillAmount = (float)Data_Player.Experience / (float)Data_Player.LvExperience;

            Data_Player.addGold(coin / 100);

            Coin.text = Data_Player.Gold.ToString();
            Exp.text = Data_Player.Experience.ToString();

            yield return new WaitForSeconds(0.02f);
        }

        if (secondExp != 0)
        {
            yield return new WaitForSeconds(0.3f);
            //여기에서 레벨 텍스트 올리고 올리고
            for (int i = 0; i < 100; i++)
            {
                Data_Player.addExperience(secondExp / 100);
                prizePercent.fillAmount = (float)Data_Player.Experience / (float)Data_Player.LvExperience;
                
                Exp.text = Data_Player.Experience.ToString();

                yield return new WaitForSeconds(0.02f);
            }
        }

        yield return new WaitForSeconds(0.3f);

        //제압 실패한 적군 집단 표시
        for (int i = 0; i < enemyGroupResult.Count; i++)
        {
            if (!enemyGroupResult[i].isSuccess)
            {
                enemyGroupResult[i].SetSuccess();
                yield return new WaitForSeconds(0.4f);
            }
        }
        yield return new WaitForSeconds(0.3f);
        //제압 실패한 만큼 현상금 떨어짐
        firstExp = 100; 
        secondExp = 0;
        
        if (Data_Player.Experience < firstExp)
            secondExp = firstExp - Data_Player.Experience;

        for (int i = 0; i < 100; i++)
        {
            Data_Player.subExperience(firstExp / 100);
            prizePercent.fillAmount = (float)Data_Player.Experience / (float)Data_Player.LvExperience;

            Exp.text = Data_Player.Experience.ToString();

            yield return new WaitForSeconds(0.02f);
        }

        if (secondExp != 0)
        {
            yield return new WaitForSeconds(0.3f);
            //여기에서 레벨 텍스트 올리고 올리고
            for (int i = 0; i < 100; i++)
            {
                Data_Player.subExperience(secondExp / 100);
                prizePercent.fillAmount = (float)Data_Player.Experience / (float)Data_Player.LvExperience;

                yield return new WaitForSeconds(0.02f);
            }
        }

        yield return new WaitForSeconds(0.3f);
        //거점 성공 여부 띄움
        SetSuccess(true); //true 아닌거 아님
        NextButton.enabled = true;
        
        yield break;
    }
}
