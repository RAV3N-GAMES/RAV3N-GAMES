using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScene : MonoBehaviour {
    public UnityEngine.UI.Text loadingPercentage;

	// Use this for initialization
	void Start () {
        StartCoroutine("LoadScene");
	}
	
    IEnumerator LoadScene()
    {
        AsyncOperation async = SceneManager.LoadSceneAsync("UI");
        async.allowSceneActivation = false;

        while (!async.isDone)
        {
            float progress = async.progress * 100.0f;
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
