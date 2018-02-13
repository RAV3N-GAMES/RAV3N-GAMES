using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMove : MonoBehaviour{
    bool isMove;

    Vector3 prePos;
    Vector3 conPos;

    ObjectColor objectColor;

    float canvasWidth;
    float canvasHeight;

    void Awake()
    {
        prePos = conPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        isMove = true;

        objectColor = GetComponent<ObjectColor>();

        canvasWidth = GameObject.Find("Canvas").GetComponent<RectTransform>().sizeDelta.x;
        canvasHeight = GameObject.Find("Canvas").GetComponent<RectTransform>().sizeDelta.y;

        StartCoroutine("OnMove");
    }

    bool isPossibleMove()
    {
        return isMove;
    }

    IEnumerator OnMove()
    {
        while (true)
        {
            prePos = conPos;

            yield return new WaitUntil(isPossibleMove);

            conPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 movePos = conPos - prePos;
            if (CameraMove())
            {
                movePos = movePos.normalized * 0.5f;
                Camera.main.transform.position += movePos;
            }
            transform.position += movePos;
        }
    }

    bool CameraMove()
    {
        if (Input.mousePosition.x < canvasWidth * 0.1f || Input.mousePosition.x > canvasWidth * 0.9f
            || Input.mousePosition.y < canvasHeight * 0.1f || Input.mousePosition.y > canvasHeight * 0.9f)
        {
            return true;
        }
        return false;
    }

    void OnMouseUp()
    {
        if (objectColor.isPossible)
        {
            isMove = false;
            GetComponent<DisplayObject>().OnDisplay();
        }
    }
}
