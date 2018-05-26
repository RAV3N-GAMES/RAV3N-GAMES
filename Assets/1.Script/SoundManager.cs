using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {
    public static SoundManager soundManager { get; private set; }

    public Dictionary<string, AudioClip> BGMList;
    public Dictionary<string, AudioClip> EffectList;

    public static AudioSource BGMAudio { get; private set; }
    public static AudioSource EffectAudio { get; private set; }

    public static bool isEffectMute;
    public static bool isBGMMute;

    public List<int> DayandNightNum;

    public ClickController BGMButton, EffectButton;

    string bgmName;

    void Awake()
    {
        if (soundManager == null)
        {
            BGMList = new Dictionary<string, AudioClip>();
            EffectList = new Dictionary<string, AudioClip>();

            AudioClip[] newBGM = Resources.LoadAll<AudioClip>("Audio/BGM") as AudioClip[];
            AudioClip[] newEffect = Resources.LoadAll<AudioClip>("Audio/Effect") as AudioClip[];

            for (int i = 0; i < newBGM.Length; i++)
                BGMList.Add(newBGM[i].name, newBGM[i]);
            for (int i = 0; i < newEffect.Length; i++)
                EffectList.Add(newEffect[i].name, newEffect[i]);

            AudioSource[] tempAudio = GetComponents<AudioSource>();

            int idx = 0;
            if (tempAudio[0].loop)
                idx = 1;

            BGMAudio = tempAudio[(idx + 1) % 2];
            EffectAudio = tempAudio[idx % 2];

            DayandNightNum = new List<int>();
            InitDayandNightNum();

            soundManager = this;

            SetOption();
        }
    }

    void SetOption()
    {
        if (PlayerPrefs.GetInt("BGM") == 1)
        {
            BGMButton.OffButtonClick();
            SetBGMMute(true);
        }
        else
        {
            BGMButton.OnButtonClick();
            SetBGMMute(false);
        }

        if (PlayerPrefs.GetInt("Effect") == 1)
        {
            EffectButton.OffButtonClick();
            SetEffectMute(true);
        }
        else
        {
            EffectButton.OnButtonClick();
            SetEffectMute(false);
        }
    }

    public void SetBGMMute(bool isMute)
    {
        isBGMMute = isMute;
        BGMAudio.mute = isBGMMute;

        if (isBGMMute)
            PlayerPrefs.SetInt("BGM", 1);
        else
            PlayerPrefs.SetInt("BGM", -1);
    }
    public void SetEffectMute(bool isMute)
    {
        isEffectMute = isMute;
        EffectAudio.mute = isEffectMute;

        if (isEffectMute)
            PlayerPrefs.SetInt("Effect", 1);
        else
            PlayerPrefs.SetInt("Effect", -1);
    }

    bool isPlayingBGM()
    {
        return BGMAudio.isPlaying;
    }

    IEnumerator DayAndNightMusic()
    {
        yield return new WaitWhile(isPlayingBGM);

        if (DayandNight.isDay)
            ChangeBGM("5_DAY");
        else
            ChangeBGM("7_NIGHT");

        yield break;
    }

    void InitDayandNightNum()
    {
        DayandNightNum.Clear();

        DayandNightNum.Add(1);
        DayandNightNum.Add(2);
        DayandNightNum.Add(3);
        DayandNightNum.Add(4);
    }

    public void ChangeBGM(string bgmName)
    {
        switch (bgmName)
        {
            case "5_DAY": case "7_NIGHT":
                int random = Random.Range(0, 100);

                if (DayandNightNum.Count == 0)
                    InitDayandNightNum();
                random = (random % DayandNightNum.Count);

                bgmName += DayandNightNum[random].ToString();

                DayandNightNum.RemoveAt(random);

                BGMAudio.loop = false;

                SetBGMAudio(bgmName);
                StartCoroutine(DayAndNightMusic());

                return;
            case "4_DAY START":
                this.bgmName = "5_DAY";
                CancelInvoke("ChangeBGM");
                InitDayandNightNum();
                Invoke("ChangeBGM", 2.5f);
                break;
            case "6_NIGHT START":
                print("night start");
                this.bgmName = "7_NIGHT"; CancelInvoke("ChangeBGM");
                InitDayandNightNum();
                Invoke("ChangeBGM", 2.5f);
                break;
            case "39_RESULT":
                BGMAudio.loop = false;
                break;
            default:
                this.bgmName = "";
                break;
        }

        SetBGMAudio(bgmName);
    }

    void ChangeBGM()
    {
        ChangeBGM(bgmName);
    }

    void SetBGMAudio(string bgmName)
    {
        foreach (var bgm in BGMList)
        {
            if (bgm.Key == bgmName)
            {
                BGMAudio.clip = bgm.Value;
                BGMAudio.Play();
                break;
            }
        }
    }
    

    public void OnEffectSound(string effectName)
    {
        foreach (var effect in EffectList)
        {
            if (effect.Key == effectName)
            {
                print(effectName);
                EffectAudio.clip = effect.Value;

                EffectAudio.Play();

                break;
            }
        }
    }
}
