using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMove : MonoBehaviour
{
    bool isMove;

    CheckTile checkTile;

    float canvasWidth;
    float canvasHeight;

    [HideInInspector]
    public Collider lastCol;

    ObjectInfo objectInfo;

    void Awake()
    {
        lastCol = null;
        isMove = true;

        checkTile = GetComponent<CheckTile>();

        canvasWidth = GameObject.Find("Canvas").GetComponent<RectTransform>().sizeDelta.x;
        canvasHeight = GameObject.Find("Canvas").GetComponent<RectTransform>().sizeDelta.y;

        StartCoroutine("OnMove");

        objectInfo = GetComponent<ObjectInfo>();
    }

    bool isPossibleMove()
    {
        bool isMouseMove = false;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.tag == "Tile" && hit.collider != lastCol)
            {
                lastCol = hit.collider;
                isMouseMove = true;
            }
        }

        return isMove & isMouseMove;
    }

    IEnumerator OnMove()
    {
        while (true)
        {
            yield return new WaitUntil(isPossibleMove);

            Vector3 movePos = lastCol.transform.position - objectInfo.pivotObject.position;
            transform.position += movePos;

            if (CameraMove())
                Camera.main.transform.position += movePos;

            //checkTile.OnCheckTile();
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

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (isMove)
            {
                if (checkTile.isPossible)
                {
                    isMove = false;
                    GetComponent<DisplayObject>().OnDisplay();
                }
                else
                {
                    isMove = false;
                    Destroy(gameObject);
                }
            }
        }
    }
}
