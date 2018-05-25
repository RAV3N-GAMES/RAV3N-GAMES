using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyGroupResult : MonoBehaviour {
    int EnemyCnt;

    public GameObject Success;
    public GameObject Fail;

    public List<Image> Enemy;
    public Image isSeizure;

    public bool isDone;
    public bool isSuccess;

    bool[] active;

    void Awake()
    {
        EnemyCnt = Enemy.Count;
    }

    public bool GetIsDone()
    {
        return isDone;
    }

	public void SetSuccess()
    {
        Success.SetActive(isSuccess);
        Fail.SetActive(!isSuccess);
    }

    IEnumerator SetEnemyActive()
    {
        for (int i = 0; i < EnemyCnt; i++)
        {
            float alpha = 1f;
            if (!active[i])
                alpha = 0.3f;

            Enemy[i].color = new Color(1, 1, 1, alpha);

            if (!active[i])
                yield return new WaitForSeconds(0.3f);
        }

        isDone = true;
        yield break;
    }

    public void InitResult(string[] enemyId, bool[] active, bool isSeizure)
    {
        this.active = active;
        EnemyCnt = Enemy.Count;

        Success.SetActive(false);
        Fail.SetActive(false);

        isSuccess = true;

        if (isSeizure)
            this.isSeizure.color = Color.blue;
        else
            this.isSeizure.color = Color.red;

        if (enemyId[0] == null)
            return;

        for (int i = 0; i < EnemyCnt; i++)
        {
            isSuccess = isSuccess & !active[i];
            Enemy[i].sprite = JsonDataManager.slotImage[enemyId[i]];
            Enemy[i].color = new Color(1, 1, 1, 1);
        }
    }
}
