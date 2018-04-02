using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyGroupResult : MonoBehaviour {
    public GameObject Success;
    public GameObject Fail;

    public List<Image> Enemy;

    public bool isDone;
    public bool isSuccess;

    bool[] active;

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
        for (int i = 0; i < active.Length; i++)
        {
            float alpha = 1f;
            if (!active[i])
                alpha = 0.3f;

            Enemy[i].color = new Color(1, 1, 1, alpha);

            yield return new WaitForSeconds(0.3f);
        }

        isDone = true;
    }

    public void InitResult(string[] enemyId, bool[] active)
    {
        this.active = active;

        Success.SetActive(false);
        Fail.SetActive(false);

        isSuccess = true;

        for (int i = 0; i < active.Length; i++)
            isSuccess = isSuccess & !active[i];

        for (int i = 0; i < enemyId.Length; i++)
        {
            Enemy[i].sprite = JsonDataManager.slotImage[enemyId[i]];
            Enemy[i].color = new Color(1, 1, 1, 1);
        }

        for(int i = enemyId.Length; i < Enemy.Count; i++)
        {
            Enemy[i].sprite = null;
            Enemy[i].color = new Color(1, 1, 1, 1);
        }
    }
}
