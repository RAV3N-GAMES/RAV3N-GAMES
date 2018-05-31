using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyDialogue : MonoBehaviour {
    public GameObject dialogueObject;
    Vector3 targetPos;
    RectTransform rectTransform;
    Text Dialogue;
    bool IsDestroyed;
    // Use this for initialization
    void Start () {
        dialogueObject = Instantiate(UIManager.current.UIDialogue);
        Dialogue = dialogueObject.GetComponent<Text>();
        Dialogue.color = Color.gray;
        dialogueObject.transform.SetParent(UIManager.current.UIDialogueParent);
        rectTransform = Dialogue.GetComponent<RectTransform>();
        rectTransform.localScale = new Vector3(5, 3, 1);
        IsDestroyed = false;
        dialogueObject.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
        if (!dialogueObject)
            return;
        targetPos = Camera.main.WorldToScreenPoint(transform.position);
        rectTransform.position = targetPos + Vector3.up * 100;
        Debug.Log("targetPos: " +targetPos);
        Debug.Log("rectTransform.position: " + rectTransform.position);
    }

    public void doDialogue() {
        Enemy e = GetComponentInChildren<Enemy>();
        StartCoroutine(activeDialogue(e, Dialogue));
        StartCoroutine(ChangePos());
    }

    IEnumerator activeDialogue(Enemy e, Text t)
    {
        dialogueObject.SetActive(true);
        t.transform.position = e.NavObj.position + new Vector3(0, 0, 1);
        if (!e.isSeizure)
            t.text = e.diaglogue[0];
        else
            t.text = e.diaglogue[2];
        yield return new WaitForSeconds(2.0f);
        IsDestroyed = true;
        dialogueObject.SetActive(false);
    }

    IEnumerator ChangePos() {
        while (!IsDestroyed) {
            if (rectTransform) { 
                targetPos = Camera.main.WorldToScreenPoint(transform.position);
                rectTransform.position = targetPos + Vector3.up * 30;
            }
            yield return null;
        }
    }
}
