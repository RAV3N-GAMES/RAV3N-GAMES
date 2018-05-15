using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MailObject : MonoBehaviour {
    [Tooltip("Mail Reward")]
    public Text MailReward;
    public int idx;

    int Reward;

    Text RewardText;
    MailBox mailBox;

    public void InitMailObject(int idx, int Reward, MailBox mailBox, Text RewardText)
    {
        this.idx = idx;
        this.Reward = Reward;

        this.mailBox = mailBox;
        this.RewardText = RewardText;

        MailReward.text = Reward.ToString();
    }

	public void OnCheckPopUp()
    {
        RewardText.text = Reward.ToString();
        mailBox.SelectedIdx = idx;

        mailBox.TakeMailPopUp.SetActive(true);
        mailBox.gameObject.SetActive(false);

        SoundManager.soundManager.OnEffectSound("8_CONTENTS");
    }
}
