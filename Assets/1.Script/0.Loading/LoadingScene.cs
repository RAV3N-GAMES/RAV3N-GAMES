using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScene : MonoBehaviour {
    public UnityEngine.UI.Text loadingPercentage;
    public GameObject ApplicationManager;

    public GameObject[] LoadingImage;

    public GameObject Managers;

    string SceneName = "";
    

	public void StartLoading(string SceneName)
    {
        int random = Random.Range(0, 100);
        random = random % 2;
        
        LoadingImage[random].SetActive(true);

        this.SceneName = SceneName;

        DontDestroyOnLoad(Managers);

        StartCoroutine("LoadScene");
    }
	
    IEnumerator LoadScene()
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(SceneName);
        async.allowSceneActivation = false;

        while (!async.isDone)
        {
            float progress = (async.progress / 0.9f) * 100.0f;
            int pRounded = Mathf.RoundToInt(progress);
            loadingPercentage.text = pRounded.ToString();

            if(async.progress >= 0.90f)
            {
                async.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
