using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskManager : MonoBehaviour
{
    public CreateObject createObject;
    public Transform CreatePopUpParent;
    public CreatePopUp createPopUp;
    public List<GameObject> Sign;
    public List<GameObject> DisplayObject;
    public GameObject Dialog;

    public Transform Mask;
    public List<Transform> ButtonList;

    public List<GameObject> TutorialTileList;

    Transform ButtonParent;

    public RoomManager roomManager;

    public bool isCompleted = false;

    public static bool isDone = true;
    [HideInInspector]
    public bool isClick = false;

    [Range(0f, 2f)]
    public float clickTime;
    [Range(0f, 2f)]
    public float arrowSpeed;

    [Range(0f, 2f)]
    public float loopArrowSpeed;

    Vector3 cameraPos;
    bool isMove = false;

    bool isClickStatus = false;

    public enum Type
    {
        Slide,
        Select,
        LongSelect,
        Slide2
    }

    int[] TaskIdx =
        { 8,    //화살표 7 -> 8
          10,   //화살표 9 -> 10
          16,   //화살표 15 -> 16
          17,   //화살표 16 -> 17
          18,   //화살표 17 -> 18
          19,   //슬라이드 하세요. -> 화살표 18 -> 19
          20,   //화살표 19 -> 20
          23,   //선택하세요 22 -> 23
          27,   //선택하세요 -> 화살표 26 -> 27
          30,   //선택하세요 -> 선택하세요 -> 화살표 29 -> 30
          32,   //선택하세요 -> 화살표 31 -> 32
          34,   //선택하세요 -> 선택하세요 -> 화살표 33 -> 34
          35,   //선택하세요 -> 화살표 34 -> 35
          36,   //선택하세요 -> 슬라이드(건물 설치) 35 -> 36
          39,   //선택하세요 -> 화살표 38 -> 39
          40,   //길게선택하세요 -> 화살표 39 -> 40
          44,   //선택하세요 -> 선택하세요 43 -> 44
          45,   //선택하세요 -> 화살표 44 -> 45
          48,   //선택하세요 -> 슬라이드(용병 설치) -> 화살표 47 -> 48
          51,   //선택하세요 50 -> 51
    };

    int conIdx;

    void Awake()
    {
        RoomManager.ChangeClickStatus(isClickStatus);
    }

    public bool OnTask(int idx)
    {
        for (int i = 0; i < TaskIdx.Length; i++)
        {
            if (TaskIdx[i] == idx)
            {
                conIdx = idx;
                StartTask(idx);
                return true;
            }
        }
        return false;
    }

    public void SetButton(bool isPossible)
    {
        if (isPossible)
        {
            isClick = false;
            ButtonParent = ButtonList[0].parent;
            ButtonList[0].SetParent(Mask);
            if (ButtonList[0].gameObject.GetComponent<SlotManager>() != null)
            {
                ButtonList[0].gameObject.GetComponent<SlotManager>().taskManager = this;
                ButtonList[0].gameObject.GetComponent<SlotManager>().isTask = true;
            }
            else
                ButtonList[0].gameObject.GetComponent<Button>().onClick.AddListener(OnClick);
        }
        else
        {
            ButtonList[0].SetParent(ButtonParent);
            if (ButtonList[0].gameObject.GetComponent<SlotManager>() != null)
            {
                ButtonList[0].gameObject.GetComponent<SlotManager>().taskManager = this;
                ButtonList[0].gameObject.GetComponent<SlotManager>().isTask = false;
            }
            else
                ButtonList[0].gameObject.GetComponent<Button>().onClick.RemoveListener(OnClick);

            ButtonList.RemoveAt(0);
        }

    }

    public void SetClickSlot(bool isPossible)
    {
        if (isPossible)
        {
            isClick = false;
            ButtonParent = ButtonList[0].parent;
            ButtonList[0].SetParent(Mask);

            ButtonList[0].gameObject.GetComponent<ClickSlot>().taskManager = this;
            ButtonList[0].gameObject.GetComponent<ClickSlot>().isTask = true;

            ButtonList[0].gameObject.GetComponent<Button>().enabled = false;

            createPopUp.transform.SetParent(Mask);
        }
        else
        {
            ButtonList[0].SetParent(ButtonParent);
        }

    }

    public void OnLongSelect()
    {
        isCompleted = true;
        Sign[(int)Type.LongSelect].SetActive(false);
    }

    public void OnSlotClick()
    {
        isClick = true;

        SetClickSlot(false);

        if (DisplayObject.Count != 0)
        {
            if (Sign[(int)Type.Slide].activeInHierarchy)
            {
                DisplayObject[0].SetActive(false);
                DisplayObject.RemoveAt(0);
            }
        }

        Sign[(int)Type.Slide].SetActive(false);
    }

    public void OnClick()
    {
        isClick = true;

        DisplayObject[0].SetActive(false);
        DisplayObject.RemoveAt(0);

        Sign[(int)Type.Select].SetActive(false);

        SetButton(false);
    }

    public void StartTask(int idx)
    {
        switch (idx)
        {
            case 8:
            case 10:
            case 16:
            case 17:
            case 18:
            case 20:
                StartCoroutine(DisplayArrow());
                break;
            case 19:
                StartCoroutine(TakeTask(Type.Slide));
                break;
            case 40:
                isClickStatus = true;
                StartCoroutine(TakeTask(Type.LongSelect));
                break;
            case 23:
            case 27:
            case 30:
            case 32:
            case 34:
            case 35:
            case 36:
            case 39:
            case 44:
            case 45:
            case 48:
            case 51:
                SetButton(true);
                StartCoroutine(ClickTask());
                break;
        }
    }

    Vector3 GetRay()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 0;
        Vector3 pos = Camera.main.ScreenToWorldPoint(mousePos);

        return pos;
    }

    bool isRightDirection()
    {
        float preDistance = (new Vector3(14.9f, 10f, 7.4f) - roomManager.prePos).magnitude;
        float postDistance = (new Vector3(14.9f, 10f, 7.4f) - roomManager.conPos).magnitude;
        if (preDistance < postDistance)
            return true;
        return false;
    }

    bool GetIsCompleted()
    {
        if (conIdx == 19)
        {
            if ((Camera.main.transform.position - new Vector3(14.9f, 10f, 7.4f)).magnitude < 2f)
                return true;
            else
            {
                if (Input.GetMouseButton(0))
                {
                    roomManager.conPos = GetRay();
                    if (isRightDirection())
                        Camera.main.transform.position += (roomManager.prePos - roomManager.conPos);
                    roomManager.prePos = roomManager.conPos;
                }
                return false;
            }
        }
        else if(conIdx == 36 || conIdx == 40 || conIdx == 48)
        {
            return isCompleted;
        }
        return true;
    }

    bool ClickStatus()
    {
        return isClick;
    }

    IEnumerator ClickTask()
    {
        isDone = false;
        Dialog.SetActive(false);

        if (DisplayObject[0].activeInHierarchy)
        {
            DisplayObject[0].SetActive(false);
            DisplayObject.RemoveAt(0);
        }

        Sign[(int)Type.Select].SetActive(true);

        while (true)
        {
            if (ClickStatus()) break;
            DisplayObject[0].SetActive(!DisplayObject[0].activeInHierarchy);
            yield return new WaitForSeconds(loopArrowSpeed);
            if (ClickStatus()) break;
        }
        if (DisplayObject.Count != 0)
            DisplayObject[0].SetActive(false);

        if (conIdx == 27 || conIdx == 32 || conIdx == 35 || conIdx == 39 || conIdx == 45)
            StartCoroutine(DisplayArrow());
        else if (conIdx == 30 || conIdx == 34)
        {
            SetButton(true);
            StartCoroutine(ClickTask());
            conIdx = -1;
        }
        else if(conIdx == 44)
        {
            SetButton(true);
            StartCoroutine(ClickTask());
            conIdx = -2;
        }
        else if(conIdx == 36 || conIdx == 48)
        {
            StartCoroutine(DisplayTask());
        }
        else if(conIdx == -1)
            StartCoroutine(DisplayArrow());
        else
        {
            yield return new WaitForSeconds(clickTime);
            Dialog.SetActive(true);
            isDone = true;
        }

        yield break;
    }

    public void SetTutorialTile()
    {
        TutorialTileList[0].SetActive(false);
        TutorialTileList.RemoveAt(0);

        createPopUp.isTutorial = false;
        isCompleted = true;
    }

    IEnumerator DisplayTask()
    {
        isMove = true;
        isDone = false;
        isCompleted = false;
        RoomManager.possibleDrag = true;
        Dialog.SetActive(false);

        TutorialTileList[0].SetActive(true);
        createPopUp.tutorialTile = TutorialTileList[0].GetComponent<TutorialTile>();
        createPopUp.isTutorial = true;

        if(conIdx == 36 || conIdx == 48)
        {
            createObject.isTutorial = true;
            createObject.taskManager = this;
            createObject.tutorialTile = TutorialTileList[0].GetComponent<TutorialTile>();
        }

        SetClickSlot(true);
        Sign[(int)Type.Slide].SetActive(true);

        if (DisplayObject[0].activeInHierarchy)
        {
            DisplayObject[0].SetActive(false);
            DisplayObject.RemoveAt(0);
        }

        while (true)
        {
            if (ClickStatus()) break;
            DisplayObject[0].SetActive(!DisplayObject[0].activeInHierarchy);
            if (ClickStatus()) break;
            yield return new WaitForSeconds(loopArrowSpeed);
        }
        if (DisplayObject.Count != 0)
            DisplayObject[0].SetActive(false);

        yield return new WaitUntil(GetIsCompleted);

        SetClickSlot(false);
        ButtonList[0].gameObject.GetComponent<ClickSlot>().isTask = false;
        ButtonList[0].gameObject.GetComponent<Button>().enabled = true;
        ButtonList.RemoveAt(0);

        if (conIdx == 36)
            createObject.isTutorial = false;

        RoomManager.possibleDrag = false;
        createPopUp.transform.SetParent(CreatePopUpParent);

        isMove = false;

        if (conIdx == 48)
        {
            createObject.isTutorial = false;
            StartCoroutine(DisplayArrow());
        }
        else
        {
            Dialog.SetActive(true);
            isDone = true;
        }

        yield break;
    }

    IEnumerator TakeTask(Type type)
    {
        isDone = false;
        isCompleted = false;
        Dialog.SetActive(false);

        if (DisplayObject[0].activeInHierarchy)
        {
            DisplayObject[0].SetActive(false);
            DisplayObject.RemoveAt(0);
        }

        for (int i = 0; i < 5; i++)
        {
            DisplayObject[0].SetActive(!DisplayObject[0].activeInHierarchy);
            yield return new WaitForSeconds(arrowSpeed);
        }

        Sign[(int)type].SetActive(true);

        if (conIdx == 19)
            cameraPos = Camera.main.transform.position;

        yield return new WaitUntil(GetIsCompleted);

        DisplayObject[0].SetActive(false);
        DisplayObject.RemoveAt(0);

        Sign[(int)type].SetActive(false);

        if (conIdx == 19)
            StartCoroutine(DisplayArrow());
        else if(conIdx == 40)
        {
            isClickStatus = false;
            StartCoroutine(DisplayArrow());
        }
        else
        {
            Dialog.SetActive(true);
            isDone = true;
        }

        yield break;
    }

    IEnumerator DisplayArrow()
    {
        isDone = false;
        Dialog.SetActive(false);
        yield return new WaitForSeconds(0.2f);

        if (DisplayObject[0].activeInHierarchy)
        {
            DisplayObject[0].SetActive(false);
            DisplayObject.RemoveAt(0);
        }

        for (int i = 0; i < 5; i++)
        {
            DisplayObject[0].SetActive(!DisplayObject[0].activeInHierarchy);
            yield return new WaitForSeconds(arrowSpeed);
        }

        Dialog.SetActive(true);
        isDone = true;

        yield break;
    }

    void Update()
    {
        if (ClickObject.isPossibleClick != isClickStatus)
            RoomManager.ChangeClickStatus(isClickStatus);
        if (isMove)
        {
            if (RoomManager.possibleDrag)
            {
                if (Input.GetMouseButton(0))
                {
                    roomManager.conPos = GetRay();
                    if (roomManager.isCameraInRoom())
                        Camera.main.transform.position += (roomManager.prePos - roomManager.conPos);
                    roomManager.prePos = roomManager.conPos;
                }
            }
        }
    }

    void LateUpdate()
    {
        if (isMove)
        {
            if (RoomManager.possibleDrag)
            {
                roomManager.conPos = GetRay();
                roomManager.prePos = roomManager.conPos;
            }
        }
    }
}