using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using LitJson;

public class MailBox : MonoBehaviour {
    public bool isSave;

    public Transform Box;
    public GameObject MailPref;

    public UnityEngine.UI.Text RewardText;

    public List<int> RewardCoinList;

    float preY;
    float postY;

    public int SelectedIdx;

    public GameObject TakeMailPopUp;
    public GameObject TakeAllMailPopUp;

    public GameObject ArrivalOfNewMail;

    void Awake()
    {
        RewardCoinList = new List<int>();

        SelectedIdx = 0;
        ReadJsonData();

        gameObject.SetActive(false);
    }


    void ReadJsonData()
    {
        string strMail;
        if (!isSave)
        {
            TextAsset textAsset = Resources.Load("Data/MailList") as TextAsset;
            strMail = textAsset.ToString();
        }
        else
        {
            try
            {
                strMail = File.ReadAllText(Application.persistentDataPath + "/MailList.json");
            }
            catch
            {
                TextAsset textAsset = Resources.Load("Data/MailList") as TextAsset;
                strMail = textAsset.ToString();
            }
        }
        JsonData mailData = JsonMapper.ToObject(strMail);

        for (int i = 0; i < mailData.Count; i++)
            RewardCoinList.Add(int.Parse(mailData[i].ToString()));
    }

    void WriteJsonData()
    {
        if (isSave)
        {
            JsonData newObj = JsonMapper.ToJson(RewardCoinList);
            File.WriteAllText(Application.persistentDataPath + "/MailList.json", newObj.ToString());
        }
    }

    void OnEnable()
    {
        PlayerPrefs.SetInt("Mail", 0);
        ArrivalOfNewMail.SetActive(false);
    }

    public void BeginDrag()
    {
        postY = preY = Input.mousePosition.y;
    }

    public void OnDrag()
    {
        postY = Input.mousePosition.y;
        Vector3 movePos = new Vector3(0, (postY - preY) / 1.5f, 0);

        Box.transform.position += movePos;

        preY = postY;
    }

    void DestroyMailList()
    {
        for(int i = 0; i < Box.childCount; i++)
        {
            Destroy(Box.GetChild(i).gameObject);
        }
    }

    public void InitMailBox()
    {
        DestroyMailList();
        Box.transform.localPosition = Vector2.zero;

        for (int i = 0; i < RewardCoinList.Count; i++)
        {
            //프리팹 생성 , 이름 idx로 변경, 부모는 박스로, this 변수로 넣어주기, 코인정보 초기화
            GameObject NewMail = Instantiate(MailPref, Box) as GameObject;

            RectTransform mailRect = NewMail.GetComponent<RectTransform>();

            mailRect.anchorMin = new Vector2(0, 1 - ((i + 1) * 0.4f));
            mailRect.anchorMax = new Vector2(1, 1 - (i * 0.4f));

            mailRect.offsetMin = Vector2.zero;
            mailRect.offsetMax = Vector2.zero;

            NewMail.GetComponent<MailObject>().InitMailObject(i, RewardCoinList[i], this, RewardText);
        }

        WriteJsonData();
    }

    public void TakeReward()
    {
        Data_Player.addGold(RewardCoinList[SelectedIdx]);
        RewardCoinList.RemoveAt(SelectedIdx);

        InitMailBox();
    }

    public void OnTakeAllPopUp()
    {
        TakeAllMailPopUp.SetActive(true);
    }

    public void TakeAll()
    {
        int Reward = 0; 
        for(int i = 0; i < RewardCoinList.Count; i++)
        {
            Reward += RewardCoinList[i];
            Data_Player.addGold(RewardCoinList[i]);
        }

        RewardCoinList.Clear();
        DestroyMailList();
        
        RewardText.text = Reward.ToString();

        WriteJsonData();
    }
}
