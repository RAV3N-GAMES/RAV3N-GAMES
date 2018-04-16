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
    
    public bool changePos;

    public GameObject WarpExit;
    public Vector3 warpPos;

    void Start()
    {
        lastCol = null;
        isMove = true;
        changePos = false;

        checkTile = GetComponent<CheckTile>();

        canvasWidth = GameObject.Find("Canvas").GetComponent<RectTransform>().sizeDelta.x;
        canvasHeight = GameObject.Find("Canvas").GetComponent<RectTransform>().sizeDelta.y;

        if (!name.Equals("Warp_Exit"))
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
        yield return null;

        if(changePos)
        {
            GetComponent<ObjectInfo>().OffDisplay();
        }

        if (GetComponent<ObjectInfo>().isDisplay)
        {
            this.enabled = false;
            yield break;
        }

        while (true)
        {
            if (isPossibleMove())
            {
                Vector3 movePos = lastCol.transform.position - objectInfo.pivotObject.position;

                transform.position += movePos;

                if (changePos)
                {
                    if (name.Equals("Warp"))
                        WarpExit.transform.position = warpPos;
                }

                yield return null;

                if (CameraMove())
                    Camera.main.transform.position += movePos;
            }
            else
                yield return null;
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
        if (Input.GetMouseButtonDown(0))
        {
            if (changePos)
            {
                isMove = true;
                GetComponent<DisplayObject>().CreateButton.GetComponent<MovePopUp>().InitObject(gameObject);

                if (name.Equals("Warp"))
                {
                    warpPos = WarpExit.transform.position;
                }

                StartCoroutine("OnMove");
            }
            else if (name.Equals("Warp_Exit"))
            {
                StartCoroutine("OnMove");
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (isMove)
            {
                isMove = false;
                changePos = false;

                GetComponent<DisplayObject>().OnDisplay();
            }
        }
    }
}