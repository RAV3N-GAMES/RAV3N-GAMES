using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskManager : MonoBehaviour
{
    public List<GameObject> WarpTutorialTile;
    public List<Transform> WarpButton;
    public List<GameObject> WarpDisplayObject;


    public GameObject ExpandMap;
    public GameObject OriginalMap;

    public CreateObject createObject;
    public Transform CreatePopUpParent;
    public CreatePopUp createPopUp;
    public List<GameObject> Sign;

    public Transform Display;
    public List<GameObject> DisplayObject;
    public GameObject Dialog;

    public Transform Mask;
    public List<Transform> ButtonList;

    public List<GameObject> TutorialTileList;

    Transform ButtonParent;
    int ButtonSiblingIdx;

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
        Slide2,
        None
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
          52,   //선택하세요 -> 선택하세요 -> 싸우고 결과창 다뜨면 끝 51 -> 52
          58,   //선택하세요 57 -> 58
          60,   //선택하세요 -> 선택하세요 -> 화살표 59 -> 60
          62,   //화살표 61 -> 62
          64,   //선택하세요 63 -> 64
          66,   //화살표 65 -> 66
          68,   //맵 확장 시 ON
          70,   //화살표 69 -> 70
          71,   //맵 확장 시 OFF
          72,   //선택하세요(방) 71 -> 72
          73,   //선택하세요 72 -> 73
          79,   //선택하세요 -> 슬라이드(워프 설치) -> 슬라이드(워프 설치)
    };

    public int conIdx;

    void Awake()
    {
        RoomManager.ChangeClickStatus(isClickStatus);

        int Cnt = Display.childCount;
        for (int i = 0; i < Cnt; i++)
            DisplayObject.Add(Display.GetChild(i).gameObject);
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
            ButtonSiblingIdx = ButtonList[0].GetSiblingIndex();

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
            ButtonList[0].SetSiblingIndex(ButtonSiblingIdx);

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
        print("SetClickSlot : " + isPossible + " btn name : " + ButtonList[0].name);
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

    public void SetDisplayWarp()
    {
        if(conIdx == -4)
        {
            StopCoroutine(DisplayWarp());

            TutorialTileList.Insert(0, WarpTutorialTile[0]);
            ButtonList.InsertRange(1, WarpButton);
            DisplayObject.InsertRange(1, WarpDisplayObject);

            conIdx = 79;
            SetButton(true);
            StartCoroutine(DisplayTask());
        }
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
            case 62:
            case 66:
            case 70:
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
            case 52:
            case 58:
            case 60:
            case 64:
            case 72:
            case 73:
            case 79:
                if (idx == 72)
                    ButtonList.Insert(0, OriginalMap.transform.GetChild(2));
                StartCoroutine(ClickTask());
                break;
            case 68:
                DisplayObject[0].SetActive(false);
                DisplayObject.RemoveAt(0);
                ExpandMap.SetActive(true);
                OriginalMap.SetActive(false);
                break;
            case 71:
                DisplayObject[0].SetActive(false);
                DisplayObject.RemoveAt(0);
                ExpandMap.SetActive(false);
                OriginalMap.SetActive(true);
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
        if (preDistance <= postDistance)
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
        else if (conIdx == 36 || conIdx == 40 || conIdx == 48 || conIdx == 79 || conIdx == -3 || conIdx == -4)
        {
            return isCompleted;
        }
        return true;
    }

    bool ClickStatus()
    {
        return isClick;
    }

    bool MouseDown()
    {
        return Input.GetMouseButtonDown(0) || Input.GetMouseButton(0);
    }

    bool isSuccessDisplayWarp()
    {
        if (Input.GetMouseButtonUp(0))
        {
            return createPopUp.tutorialTile.isSuccess();
        }
        else
            return false;
    }

    public IEnumerator DisplayWarp()
    {
        isMove = true;
        isDone = false;
        isCompleted = false;

        RoomManager.possibleDrag = true;
        Dialog.SetActive(false);

        TutorialTileList[0].SetActive(true);
        createPopUp.tutorialTile = TutorialTileList[0].GetComponent<TutorialTile>();
        createPopUp.isTutorial = true;

        SetClickSlot(true);
        Sign[(int)Type.Slide].SetActive(true);

        if (DisplayObject[0].activeInHierarchy)
        {
            DisplayObject[0].SetActive(false);
            DisplayObject.RemoveAt(0);
        }

        while (true)
        {
            if (MouseDown()) break;
            DisplayObject[0].SetActive(!DisplayObject[0].activeInHierarchy);
            if (MouseDown()) break;
            yield return new WaitForSeconds(loopArrowSpeed);
        }

        if (DisplayObject.Count != 0)
            DisplayObject[0].SetActive(false);
        OnSlotClick();

        RoomManager.possibleDrag = false;

        yield return new WaitUntil(isSuccessDisplayWarp);
        SetClickSlot(false);

        ButtonList[0].gameObject.GetComponent<ClickSlot>().isTask = false;
        ButtonList[0].gameObject.GetComponent<Button>().enabled = true;
        ButtonList.RemoveAt(0);

        if (conIdx == 36)
            createObject.isTutorial = false;
        createPopUp.transform.SetParent(CreatePopUpParent);

        isMove = false;
        conIdx = -5;
        StartCoroutine(ClickTask());

        print("DisplayWarp");
        yield break;
    }

    IEnumerator ClickTask()
    {
        SetButton(true);

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
        else if (conIdx == 30 || conIdx == 34 || conIdx == 60)
        {
            if (conIdx == 60)
                yield return new WaitForSeconds(2.3f);
            StartCoroutine(ClickTask());
            conIdx = -1;
        }
        else if (conIdx == 52)
        {
            StartCoroutine(ClickTask());
            conIdx = -3;
        }
        else if (conIdx == 44)
        {
            StartCoroutine(ClickTask());
            conIdx = -2;
        }
        else if (conIdx == 36 || conIdx == 48 || conIdx == 79)
        {
            StartCoroutine(DisplayTask());
        }
        else if (conIdx == -1)
            StartCoroutine(DisplayArrow());
        else if (conIdx == -3)
            StartCoroutine(TakeTask(Type.None));
        else if (conIdx == -4)
            StartCoroutine(DisplayWarp());
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
        if (conIdx != 79)
        {
            TutorialTileList[0].SetActive(false);
            TutorialTileList.RemoveAt(0);
        }
        createPopUp.isTutorial = false;
        isCompleted = true;
    }

    public IEnumerator DisplayTask()
    {
        isMove = true;
        isDone = false;
        isCompleted = false;
        RoomManager.possibleDrag = true;
        Dialog.SetActive(false);

        TutorialTileList[0].SetActive(true);
        createPopUp.tutorialTile = TutorialTileList[0].GetComponent<TutorialTile>();
        createPopUp.isTutorial = true;

        if (conIdx == 36 || conIdx == 48 || conIdx == 79)
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
        OnSlotClick();

        RoomManager.possibleDrag = false;
        yield return new WaitUntil(GetIsCompleted);
        SetClickSlot(false);
        
        ButtonList[0].gameObject.GetComponent<ClickSlot>().isTask = false;
        ButtonList[0].gameObject.GetComponent<Button>().enabled = true;
        ButtonList.RemoveAt(0);

        if (conIdx == 36)
            createObject.isTutorial = false;
        createPopUp.transform.SetParent(CreatePopUpParent);

        isMove = false;

        if (conIdx == 48)
        {
            createObject.isTutorial = false;
            StartCoroutine(DisplayArrow());
        }
        else if (conIdx == 79)
        {
            conIdx = -4;
            StartCoroutine(ClickTask());
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

        if (type != Type.None)
            Sign[(int)type].SetActive(true);
        for (int i = 0; i < 3; i++)
        {
            DisplayObject[0].SetActive(!DisplayObject[0].activeInHierarchy);
            yield return new WaitForSeconds(arrowSpeed);
        }

        //if (conIdx == 19)
        //    cameraPos = Camera.main.transform.position;

        //////////////////////////임시//////////////////////
        if (conIdx == -3)
        {
            yield return new WaitForSeconds(1f);
            FindObjectOfType<DayandNight>().changeState();
        }
        ////////////////////////////////////////////////////

        if (conIdx == 19)
        {
            roomManager.prePos = GetRay();
            roomManager.conPos = GetRay();
        }
        yield return new WaitUntil(GetIsCompleted);

        DisplayObject[0].SetActive(false);
        DisplayObject.RemoveAt(0);

        if (type != Type.None)
            Sign[(int)type].SetActive(false);

        if (conIdx == 19)
            StartCoroutine(DisplayArrow());
        else if (conIdx == 40)
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
        print("displayArrow");
        isDone = false;
        Dialog.SetActive(false);
        yield return new WaitForSeconds(0.2f);

        if (DisplayObject[0].activeInHierarchy)
        {
            DisplayObject[0].SetActive(false);
            DisplayObject.RemoveAt(0);
        }

        for (int i = 0; i < 3; i++)
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