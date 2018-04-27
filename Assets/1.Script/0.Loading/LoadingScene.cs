using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScene : MonoBehaviour {
    public UnityEngine.UI.Text loadingPercentage;
    public GameObject ApplicationManager;

    public GameObject[] LoadingImage;

	public void StartLoading()
    {
        int random = Random.Range(0, 100);
        random = random % 2;

        LoadingImage[random].SetActive(true);

        StartCoroutine("LoadScene");
    }
	
    IEnumerator LoadScene()
    {
        AsyncOperation async = SceneManager.LoadSceneAsync("UI");
        async.allowSceneActivation = false;

        while (!async.isDone)
        {
            float progress = (async.progress / 0.9f) * 100.0f;
            int pRounded = Mathf.RoundToInt(progress);
            loadingPercentage.text = pRounded.ToString();

            if(async.progress >= 0.9f)
            {
                async.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
