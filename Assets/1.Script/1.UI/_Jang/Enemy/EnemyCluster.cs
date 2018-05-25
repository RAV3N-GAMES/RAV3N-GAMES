using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyCluster : MonoBehaviour {
    public List<Enemy> eList;
    public List<Friendly> GroupNearFriend;
    public List<Trap> GroupNearTrap;
    public List<SecretActs> GroupNearSecret;
    public List<Wall> GroupNearWall;

    public Friendly targetFriend;
    public Enemy healTarget;
    public Trap targetTrap;
    public SecretActs targetSecret;
    public Wall targetWall;
    public int roomPassCount;
    public int EnterOrder;

	// Use this for initialization
	void Awake () {
        eList = new List<Enemy>();
        GroupNearFriend = new List<Friendly>();
        GroupNearTrap = new List<Trap>();
        GroupNearSecret = new List<SecretActs>();
        GroupNearWall = new List<Wall>();

        targetFriend = null;
        targetWall = null;
        healTarget = null;
        targetTrap = null;
        targetSecret = null;
        roomPassCount = 0;
        EnterOrder = 0;
    }

    // Update is called once per frame
    void Update () {
        if (targetFriend)
            clusterSetNextTargetFriend(targetFriend);
        if (targetTrap)
            clusterSetNextTargetTrap(targetTrap);
        if (targetWall)
            clusterSetNextTargetWall(targetWall);
        clusterSetNextTargetSecret();
        EnterCheck();
    }

    public void clusterSetNextIdx(int idx)
    {
        for (int i = 0; i < eList.Count; i++) {
            eList[i].nextIdx = idx;
        }
    }

    public void clusterSetNextTargetFriend(Friendly f) {
        for (int i = 0; i < eList.Count; i++)
        {
            eList[i].targetFriend= f;
        }
    }

    public void clusterSetNextTargetSecret() {
        if (eList.Count > 0)
            targetSecret = eList[0].FindClosestSecret(eList[0].SetYZero(eList[0].NavObj));

        for (int i = 0; i < eList.Count; i++)
        {
            eList[i].targetSecret = targetSecret;
        }
    }

    public void clusterSetNextTargetTrap(Trap t) {
        for (int i = 0; i < eList.Count; i++)
        {
            if (eList[i].name.Equals("MonsterPickPocket2D"))
            {
                EnemyPickPocket ep = (EnemyPickPocket)eList[i];
                ep.targetTrap = t;
            }
        }
    }

    public void clusterSetNextTargetWall(Wall w) {
        for (int i = 0; i < eList.Count; i++)
        {
            if (!eList[i].name.Equals("MonsterPickPocket2D"))
                eList[i].targetWall = w;
        }
    }

    public void clusterStole(SecretActs s) {
        if(eList.Count>0)
            StartCoroutine(eList[0].StealEvent());
    }

    public void clusterEscape() {
        for (int i = 0; i < eList.Count; i++)
        {
            StartCoroutine(eList[i].EscapeEvent());
        }
    }

    public Enemy HurtEnemy() {
        if (eList.Count <= 0)
            return null;
        Enemy e=eList[0];
        for (int i = 1; i < eList.Count; i++) {
            if (e.Hp > eList[i].Hp) {
                e = eList[i];
            }
        }
        
        if (e.Hp >= e.MaxHp)//모두 만피면 그냥 갈길 가도록 null return
            return null;
        else 
            return e;
    }

    public bool isGathered() {
        bool result = true;

        for (int i = 0; i < eList.Count; i++) {
            Enemy e = eList[i];
            if (Vector3.Distance(e.GetComponentInChildren<EnemyBody>().transform.position, GameManager.current.enemyGroups[e.PresentRoomidx].ExitPoint[e.Exitdirection].transform.position) > e.enemyAI.stoppingDistance) { 
                result = false;
                break;
            }
        }
        return result;
    }

    public void MoveToNextRoom(int nextidx, int ExitDirection) {
        int nextroomEnterPoint = -1;
        if (ExitDirection == 1)
        {
            nextroomEnterPoint = 3;
        }
        else {
            nextroomEnterPoint = Mathf.Abs(ExitDirection - 2);
        }
        StartCoroutine(WaitBattleEnd(nextidx, nextroomEnterPoint));
    }

    public bool IsBattle() {
        bool result = false;
        for (int i = 0; i < eList.Count; i++) {
            if (eList[i].targetFriend && eList[i].IsNear(eList[i].NavObj, eList[i].targetFriend.transform))
            {
                result = true;
                break;
            }
            else if (eList[i].name.Equals("MonsterPickPocket2D")) {
                EnemyPickPocket Epp = (EnemyPickPocket)eList[i];
                if (Epp.targetTrap) {
                    result = true;
                    break;
                }
            }
        }
        return result;
    }
    IEnumerator WaitBattleEnd(int nextidx, int nextroomEnterPoint) {
        yield return new WaitUntil(IsBattle);
        for (int i = 0; i < eList.Count; i++)
        {
            eList[i].enemyAI.SetDestination(eList[i].SetYZero(GameManager.current.enemyGroups[nextidx].ExitPoint[nextroomEnterPoint]));
        }
    }

    public void SetOrderFriendly()
    {
        if (GroupNearFriend.Count > 1)
        {
            GroupNearFriend.Sort(
                delegate (Friendly f1, Friendly f2)
                {
                    if (f1 == null)
                        return 1;
                    else if (f2 == null)
                        return -1;
                    else
                    {
                        float d1, d2;
                        d1 = Vector3.Distance(f1.transform.position, transform.position);
                        d2 = Vector3.Distance(f2.transform.position, transform.position);
                        if (d1 > d2)
                            return 1;
                        else if (d1 < d2)
                            return -1;
                        else
                            return 0;
                    }
                });
        }
        if (GroupNearFriend.Count > 0)
        {
            targetFriend = GroupNearFriend[0];
        }
        else
            targetFriend = null;
    }
    
    public void SetOrderTrap()
    {
        for (int i = 0; i < GroupNearTrap.Count; i++)
        {
            if (!GroupNearTrap[i].transform.parent.gameObject.activeSelf)
            {
                GroupNearTrap.Remove(GroupNearTrap[i]);
                i--;
            }
        }

        if (GroupNearTrap.Count > 1)
        {
            GroupNearTrap.Sort(
                delegate (Trap t1, Trap t2)
                {
                    if (t1 == null)
                        return 1;
                    else if (t2 == null)
                        return -1;
                    else
                    {
                        float d1, d2;
                        d1 = Vector3.Distance(t1.transform.position, transform.position);
                        d2 = Vector3.Distance(t2.transform.position, transform.position);
                        if (d1 > d2)
                            return 1;
                        else if (d1 < d2)
                            return -1;
                        else
                            return 0;
                    }
                });
        }

        if (GroupNearTrap.Count > 0)
            targetTrap = GroupNearTrap[0];
        else
            targetTrap = null;
    }
    public void SetOrderWall()
    {
        if (GroupNearWall.Count > 1)
        {
            GroupNearWall.Sort(
                delegate (Wall w1, Wall w2)
                {
                    if (w1 == null)
                        return 1;
                    else if (w2 == null)
                        return -1;
                    else
                    {
                        float d1, d2;
                        d1 = Vector3.Distance(w1.transform.position, transform.position);
                        d2 = Vector3.Distance(w2.transform.position, transform.position);
                        if (d1 > d2)
                            return 1;
                        else if (d1 < d2)
                            return -1;
                        else
                            return 0;
                    }
                });
        }

        if (GroupNearWall.Count > 0)
            targetWall = GroupNearWall[0];
        else
            targetWall = null;
    }

    private void EnterCheck() {
        bool IsAllEntered = true;
        for (int i = 0; i < eList.Count; i++) {
            if (!eList[i].isEntered)
            {
                IsAllEntered = false;
                break;
            }
        }

        if (eList.Count <= 0)
            IsAllEntered = false;

        if (IsAllEntered) {
            for (int i = 0; i < eList.Count; i++)
            {
                eList[i].isEntered = false;
            }

            try
            {
                 for (int i = 0; i < eList.Count; i++) {
                    if (eList[i].EnterOrder == roomPassCount) {
                        eList[i].GetComponentInParent<EnemyDialogue>().doDialogue();
                        break;
                    }
                }
            }
            catch { }
            roomPassCount++;
        }
    }
}