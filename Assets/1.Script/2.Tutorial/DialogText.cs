using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogText : MonoBehaviour {
    public Text[] Line;
    public GameObject NextButton;

    [Range(0f, 1f)]
    public float speed;

    void OnDisable()
    {
        InitText();
    }

    public void InitText()
    {
        for (int i = 0; i < Line.Length; i++)
            Line[i].text = "";
    }

	public void ChangeText(int idx, string contents)
    {
        if (idx < Line.Length)
            Line[idx].text = contents;
        else
            Debug.Log("line over");
    }

    public void OnNextButton()
    {
        if (gameObject.activeInHierarchy)
            StartCoroutine(AnimateNextButton());
        else
            InitText();
    }

    IEnumerator AnimateNextButton()
    {
        while(true)
        {
            NextButton.SetActive(!NextButton.activeInHierarchy);
            yield return new WaitForSeconds(speed);
        }
    }

    public void OffNextButton()
    {
        StopCoroutine(AnimateNextButton());
        NextButton.SetActive(false);
    }
}
