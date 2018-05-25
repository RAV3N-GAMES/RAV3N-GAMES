using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultPopUp : MonoBehaviour
{
    const int EnemyKill_ExpReward = 5;
    const int EnemyGroupKill_ExpReward = 10;
    const int Fail_Exp = -30;

    bool isSuccess;

    public Button NextButton;
    public GameObject DamageReport;
    public List<GameObject> EnemyGroupList;

    public GameObject Success;
    public GameObject Fail;

    public Image prizePercent;
    public Text Fame;
    public Text Coin;
    public Text Exp;

    public GameObject ArrivalOfNewMail;
    public MailBox mailBox;

    public RoomManager roomManager;
    public TaskManager taskManager;

    List<EnemyGroupResult> enemyGroupResult = new List<EnemyGroupResult>();

    int EnemyCnt;
    int DiedEnemeyCnt;

    //제압한 적에 대한 보상
    int RewardEnemy_Gold;
    int RewardEnemy_Exp;

    //제압한 적 집단에 대한 보상
    int RewardGroup_Gold;
    int RewardGroup_Exp;

    //목적 달성한 집단에 대한 패배
    int RewardFail_Exp;


    float[][] RewardCoefficient = { new float[] {1f, 0.7f, 0.4f, 0.1f},
                                    new float[] {1f, 0.869f, 0.738f, 0.607f, 0.476f, 0.345f, 0.214f, 0.08f},
                                    new float[] {1f, 0.914f, 0.828f, 0.742f, 0.656f, 0.57f, 0.484f, 0.398f, 0.312f, 0.226f, 0.14f, 0.05f}};

    void OnEnable()
    {
        SoundManager.soundManager.ChangeBGM("39_RESULT");
    }

    void SetEnemyListActive(int enemyNum) //적군 집단 수
    {
        EnemyCnt = enemyNum;

        EnemyGroupList[0].SetActive(false);
        EnemyGroupList[1].SetActive(false);
        EnemyGroupList[2].SetActive(false);

        EnemyGroupList[enemyNum - 1].SetActive(true);
    }

    void InitEnemyGroupResult()
    {
        int createdEnemyCnt = DayandNight.CreatedEnemy.Count;
        int[] GroupId = new int[EnemyCnt];
        int GroupCnt = 0;

        for (int i = 0; i < DayandNight.CreatedEnemy.Count; i++)
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


        int SeizureCnt = 0;
        bool isStolen = true;

        for (int i = 0; i < EnemyCnt; i++)
        {
            string[] enemyId = new string[4];
            bool[] enemyActive = new bool[4];

            int Cnt = 0;
            bool isSeizure = false;
            for (int j = 0; j < createdEnemyCnt; j++)
            {
                if (DayandNight.CreatedEnemy[j].Group == GroupId[i])
                {
                    enemyId[Cnt] = DayandNight.CreatedEnemy[j].name;
                    enemyActive[Cnt] = !DayandNight.CreatedEnemy[j].isDie;

                    if (DayandNight.CreatedEnemy[j].isDie)
                        DiedEnemeyCnt++;
                    else
                    {
                        if (DayandNight.CreatedEnemy[j].isSeizure)
                        {
                            isStolen = isStolen & DayandNight.CreatedEnemy[j].isStolen;
                        }
                    }
                    if (DayandNight.CreatedEnemy[j].isSeizure)
                    {
                        isSeizure = true;
                        SeizureCnt++;
                    }
                    else
                        isSeizure = false;
                    if (DayandNight.CreatedEnemy[j].isDefeated)
                        isSuccess = false;

                    if (Cnt == 3)
                        break;
                    else
                        Cnt++;
                }
            }
            enemyGroupResult[i].InitResult(enemyId, enemyActive, isSeizure);
        }

        if (isSuccess)
        {
            if (SeizureCnt == DayandNight.CreatedEnemy.Count)
            {
                //모든 집단이 다 탈취 성공했는지 여부 판단
                isSuccess = !isStolen;
            }
        }
    }



    public void InitResultPopUp()
    {
        ApplicationManager.isPossible = false;

        enemyGroupResult.Clear();

        if (EnemyGroupList.Count == 1)
        {
            isSuccess = true;
            enemyGroupResult.Add(EnemyGroupList[0].transform.GetChild(0).GetComponent<EnemyGroupResult>());
            enemyGroupResult[0].InitResult(new string[] { "MonsterFlyTeen2D" }, new bool[] { false }, true);

            PlayerExp = Data_Player.Experience;
            PlayerCoin = Data_Player.Gold;

            SetUI(0, 0);
            StartCoroutine(TutorialResult());
        }
        else
        {
            isSuccess = false;

            Success.SetActive(false);
            Fail.SetActive(false);

            NextButton.enabled = false;

            Exp.text = Data_Player.Experience.ToString();
            Coin.text = Data_Player.Gold.ToString();
            Fame.text = Data_Player.Fame.ToString();

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

            if (DayandNight.CreatedEnemy.Count < 4)
            {
                gameObject.SetActive(false);
                return;
            }

            InitEnemyGroupResult();

            PlayerExp = Data_Player.Experience;
            PlayerCoin = Data_Player.Gold;

            RewardEnemy_Gold = 0; RewardEnemy_Exp = 0;
            RewardGroup_Gold = 0; RewardGroup_Exp = 0;
            RewardFail_Exp = 0;

            SetPlayerInfo();

            if (!isSuccess)
                SendMail();

            StartCoroutine("PlayResult");
        }
    }



    void SetPlayerInfo()
    {
        //제압 성공한 적군에 대한 보상
        RewardEnemy_Gold = DiedEnemeyCnt * ResourceManager_Player.Tbl_Player[Data_Player.Fame - 4].Reward_Kill;
        RewardEnemy_Exp = DiedEnemeyCnt * EnemyKill_ExpReward;

        Data_Player.addGold(RewardEnemy_Gold);
        Data_Player.addExperience(RewardEnemy_Exp);

        //제압 성공 집단에 대한 보상
        int SuccessCnt = 0;
        for (int i = 0; i < enemyGroupResult.Count; i++)
        {
            if (enemyGroupResult[i].isSuccess)
                SuccessCnt++;
        }
        RewardGroup_Gold = SuccessCnt * ResourceManager_Player.Tbl_Player[Data_Player.Fame - 4].Reward_GroupOppression;
        RewardGroup_Exp = SuccessCnt * EnemyGroupKill_ExpReward;

        Data_Player.addGold(RewardGroup_Gold);
        Data_Player.addExperience(RewardGroup_Exp);

        //목적 달성한 집단에 대한 피해
        //목적 달성한 집단 수 곱해줘야함
        RewardFail_Exp = Fail_Exp;
        if (Data_Player.Experience < -RewardFail_Exp)
            RewardFail_Exp = -Data_Player.Experience;

        Data_Player.subExperience(-RewardFail_Exp);

        SoundManager.EffectAudio.clip = null;
    }

    void SetSuccess(bool isSuccess)
    {
        SoundManager.soundManager.OnEffectSound("44_RESULT STAMP");

        Success.SetActive(isSuccess);
        Fail.SetActive(!isSuccess);
    }

    int GetReward()
    {
        int rewardCoin = 0;

        //파괴에 대한 보상
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < DamageReportPopUp.Building[i]; j++)
                rewardCoin += DamageReportPopUp.BuildingLevel[i][j];
        }

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < DamageReportPopUp.OurForces[i]; j++)
                rewardCoin += DamageReportPopUp.OurForcesLevel[i][j];
        }

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < DamageReportPopUp.Trap[i]; j++)
                rewardCoin += DamageReportPopUp.TrapLevel[i][j];
        }

        //수리에 대한 보상
        rewardCoin += roomManager.GetAllRepairCost();
        if (DiedEnemeyCnt < EnemyCnt * 4)
            rewardCoin = (int)(rewardCoin * RewardCoefficient[EnemyCnt - 1][DiedEnemeyCnt]);
        return rewardCoin;
    }

    void SendMail()
    {
        int rewardMail = GetReward();

        if (rewardMail > 0)
        {
            PlayerPrefs.SetInt("Mail", 1);

            mailBox.RewardCoinList.Add(rewardMail);
            mailBox.InitMailBox();

            DiedEnemeyCnt = 0;

            ArrivalOfNewMail.SetActive(true);
        }
    }

    int PlayerExp;  //결과 반영 전 Exp
    int PlayerCoin; //결과 반영 전 Gold

    int Reward_Min = 0, Reward_Max = 0;

    void SetPlayerReward(int Exp)
    {
        int Fame = ResourceManager_Player.GetPlayerFame(Exp);

        Reward_Min = ResourceManager_Player.Tbl_Player[Fame - 4].Reward_Min;
        Reward_Max = ResourceManager_Player.Tbl_Player[Fame - 4].Reward_Max;
    }

    void SetUI(float PlusExp, float PlusCoin)
    {
        int newFame = ResourceManager_Player.GetPlayerFame((int)PlusExp + PlayerExp);
        if (newFame.ToString() != Fame.text)
        {
            if (int.Parse(Fame.text) < newFame)
                SoundManager.soundManager.ChangeBGM("46_SECURITY LEVEL");
            Fame.text = newFame.ToString();
            SetPlayerReward((int)PlusExp + PlayerExp);
        }

        prizePercent.fillAmount = ((PlusExp + PlayerExp) - Reward_Min) / (Reward_Max - Reward_Min);

        Coin.text = ((int)PlusCoin + PlayerCoin).ToString();
        Exp.text = ((int)PlusExp + PlayerExp).ToString();
    }

    void SetPlayerData(int Gold, int Exp)
    {
        PlayerCoin += Gold;
        PlayerExp += Exp;

        SetPlayerReward(PlayerExp);
        SetUI(0, 0);
    }

    IEnumerator TutorialResult()
    {
        //없앤 적군 표시
        enemyGroupResult[0].isDone = false;
        enemyGroupResult[0].StartCoroutine("SetEnemyActive");

        yield return new WaitUntil(enemyGroupResult[0].GetIsDone);
        yield return new WaitForSeconds(1f);

        //제압 성공한 적군 집단 표시
        SoundManager.soundManager.OnEffectSound("40_RESULT SIGN");
        enemyGroupResult[0].SetSuccess();
        yield return new WaitForSeconds(1f);

        //거점 성공 여부 띄움
        SetSuccess(isSuccess);
        NextButton.enabled = true;

        yield return new WaitForSeconds(1f);
        taskManager.isCompleted = true;

        yield break;
    }

    IEnumerator PlayResult()
    {
        //없앤 적군 표시
        for (int i = 0; i < enemyGroupResult.Count; i++)
        {
            enemyGroupResult[i].isDone = false;
            enemyGroupResult[i].StartCoroutine("SetEnemyActive");

            yield return new WaitUntil(enemyGroupResult[i].GetIsDone);
        }

        //없앤 적군만큼 현상금 + 달러오르고
        if (DiedEnemeyCnt != 0)
        {
            yield return new WaitForSeconds(0.3f);

            SoundManager.soundManager.OnEffectSound("41_RESULT UP");

            float PlusExp = 0;
            float PlusCoin = 0;

            SetPlayerReward(PlayerExp);

            for (int i = 0; i < 100; i++)
            {
                PlusExp += RewardEnemy_Exp / 100f;
                PlusCoin += RewardEnemy_Gold / 100f;

                SetUI(PlusExp, PlusCoin);

                yield return new WaitForSeconds(0.02f);
            }

            SetPlayerData(RewardEnemy_Gold, RewardEnemy_Exp);

            yield return new WaitForSeconds(0.3f);
        }

        //제압 성공한 적군 집단 표시
        SoundManager.soundManager.OnEffectSound("40_RESULT SIGN");
        for (int i = 0; i < enemyGroupResult.Count; i++)
        {
            if (enemyGroupResult[i].isSuccess)
            {
                enemyGroupResult[i].SetSuccess();
                yield return new WaitForSeconds(0.4f);
            }
        }

        if (RewardGroup_Exp != 0)
        {
            yield return new WaitForSeconds(0.3f);

            SoundManager.soundManager.OnEffectSound("41_RESULT UP");

            float PlusExp = 0;
            float PlusCoin = 0;

            SetPlayerReward(PlayerExp);
            for (int i = 0; i < 100; i++)
            {
                PlusExp += RewardGroup_Exp / 100f;
                PlusCoin += RewardGroup_Gold / 100f;

                SetUI(PlusExp, PlusCoin);

                yield return new WaitForSeconds(0.02f);
            }
            SetPlayerData(RewardGroup_Gold, RewardGroup_Exp);

            yield return new WaitForSeconds(0.3f);
        }
        yield return new WaitForSeconds(0.3f);

        //목적 달성한 적군 집단 표시
        for (int i = 0; i < enemyGroupResult.Count; i++)
        {
            if (!enemyGroupResult[i].isSuccess)
            {
                enemyGroupResult[i].SetSuccess();
                yield return new WaitForSeconds(0.4f);
            }
        }

        if (RewardFail_Exp != 0)
        {
            yield return new WaitForSeconds(0.3f);

            SoundManager.soundManager.OnEffectSound("42_RESULT DOWN");

            float PlusExp = 0;

            SetPlayerReward(PlayerExp);
            for (int i = 0; i < 100; i++)
            {
                PlusExp += RewardFail_Exp / 100f;
                SetUI(PlusExp, 0);

                yield return new WaitForSeconds(0.02f);
            }

            SetPlayerData(0, RewardFail_Exp);

            yield return new WaitForSeconds(0.3f);
        }
        //거점 성공 여부 띄움
        SetSuccess(isSuccess);
        NextButton.enabled = true;

        yield break;
    }

    void OnDisable()
    {
        SoundManager.soundManager.OnEffectSound("43_RESULT CHANGE");
    }
}