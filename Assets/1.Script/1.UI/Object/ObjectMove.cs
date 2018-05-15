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

    public bool isNewObj = false;

    void Awake()
    {
        lastCol = null;
        isMove = true;
        changePos = false;
    }

    void Start()
    {
        checkTile = GetComponent<CheckTile>();

        canvasWidth = GameObject.Find("Canvas").GetComponent<RectTransform>().sizeDelta.x * GameObject.Find("Canvas").GetComponent<RectTransform>().localScale.x;
        canvasHeight = GameObject.Find("Canvas").GetComponent<RectTransform>().sizeDelta.y * GameObject.Find("Canvas").GetComponent<RectTransform>().localScale.y;

        if (!name.Equals("Warp_Exit") && !isNewObj)
        {
            StartCoroutine("OnMove");
        }

        objectInfo = GetComponent<ObjectInfo>();
    }

    bool isPossibleMove()
    {
        bool isMouseMove = false;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.tag == "Tile" || hit.collider.tag == "Door")
            {
                if (hit.collider != lastCol)
                {
                    lastCol = hit.collider;
                    isMouseMove = true;
                }
            }
        }

        return isMove & isMouseMove;
    }

    bool isPossibleStart()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.tag == "Tile" || hit.collider.tag == "Door")
            {
                transform.position = hit.collider.transform.position;
                return true;
            }
        }

        return false;
    }

    public IEnumerator ArrayObject()
    {
        yield return new WaitUntil(isPossibleStart);
        yield return null;

        Camera.main.transform.position = new Vector3( transform.position.x, Camera.main.transform.position.y, transform.position.z);

        if (!name.Equals("Warp_Exit"))
            StartCoroutine("OnMove");
    }


    IEnumerator OnMove()
    {
        yield return null;

        if (changePos)
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
            yield return new WaitUntil(isPossibleMove);

            Vector3 movePos = lastCol.transform.position - objectInfo.pivotObject.position;
            transform.position += movePos;

            if (CameraMove())
            {
                Camera.main.transform.position += movePos;
            }
            yield return null;

            if (changePos)
            {
                if (name.Equals("Warp"))
                    WarpExit.transform.position = warpPos;
            }

            checkTile.OnCheckTile();
            yield return null;
        }
    }

    bool CameraMove()
    {
        if (Input.mousePosition.x < canvasWidth * 0.15f || Input.mousePosition.x > canvasWidth * 0.85f
            || Input.mousePosition.y < canvasHeight * 0.15f || Input.mousePosition.y > canvasHeight * 0.85f)
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