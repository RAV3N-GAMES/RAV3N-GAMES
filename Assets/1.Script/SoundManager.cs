using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {
    public static SoundManager soundManager { get; private set; }

    public Dictionary<string, AudioClip> BGMList;
    public Dictionary<string, AudioClip> EffectList;

    public static AudioSource BGMAudio { get; private set; }
    public static AudioSource EffectAudio { get; private set; }

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

            soundManager = this;
        }
    }

    public void OnOffBGM(bool isPossible)
    {
        BGMAudio.mute = !isPossible;
    }
    public void OnOffEffectSound(bool isPossible)
    {
        EffectAudio.mute = !isPossible;
    }

    public void ChangeBGM(string bgmName)
    {
        switch (bgmName)
        {
            case "5_DAY": case "7_NIGHT":
                int random = Random.Range(0, 100);
                random = (random % 4) + 1;
                bgmName += random.ToString();

                BGMAudio.loop = true;
                break;
            case "4_DAY START":
                this.bgmName = "5_DAY"; CancelInvoke("ChangeBGM"); Invoke("ChangeBGM", 2.5f);
                break;
            case "6_NIGHT START":
                this.bgmName = "7_NIGHT"; CancelInvoke("ChangeBGM"); Invoke("ChangeBGM", 2.5f);
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
