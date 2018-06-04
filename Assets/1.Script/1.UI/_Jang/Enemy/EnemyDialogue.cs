using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyDialogue : MonoBehaviour {
    public GameObject dialogueObject;
    Vector3 targetPos;
    RectTransform rectTransform;
    Text Dialogue;
    Image BackGround;

    bool IsDestroyed;
    // Use this for initialization
    void Start () {
        dialogueObject = Instantiate(UIManager.current.UIDialogue);
        Dialogue = dialogueObject.GetComponent<Text>();
        BackGround = dialogueObject.GetComponentInChildren<Image>();
        Dialogue.color = Color.white;
        dialogueObject.transform.SetParent(UIManager.current.UIDialogueParent);
        rectTransform = Dialogue.GetComponent<RectTransform>();
        rectTransform.localScale = new Vector3(5, 3, 1);
        IsDestroyed = false;
        dialogueObject.SetActive(false);
        BackGround.rectTransform.position = Vector3.zero;
        BackGround.rectTransform.localScale = new Vector3(1.6f, 0.3f, 1.0f);
    }
	
	// Update is called once per frame
	void Update () {
        if (!dialogueObject)
            return;
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
                BackGround.rectTransform.localPosition = Vector3.zero;
                targetPos = Camera.main.WorldToScreenPoint(transform.position);
                rectTransform.position = targetPos + Vector3.up * 230;
            }
            yield return null;
        }
    }
}
