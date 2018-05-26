using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskObject : MonoBehaviour {
    public List<TaskType> TaskList;
    public int Idx;
    public List<GameObject> DisplayObject;
    public List<Transform> ButtonList;
    public List<GameObject> TutorialTile;

    Transform ButtonParent;
    int ButtonSiblingIdx;

    int DisplayIdx = 0;
    int ButtonIdx = 0;
    int TutorialTileIdx = 0;

    bool isDone = false;
    int Cnt = 0;

    bool isClick = false; //Select 용 & Display시 Slot 눌렀는지 여부 판단
    bool isLongClick = false; //LongSelect 용

    bool isDisplay = false; //내려놓았을 때 성공한 위치인지

    bool isResult = false;

    RoomManager roomManager;
    DayandNight dayAndNight;
    ResultPopUp resultPopUp;

    public enum TaskType
    {
        Arrow,
        Select,
        LongSelect,
        Display,
        MoveMap,
        Fight
    }

    bool IsDone()
    {
        return isDone;
    }

    public void InitTask(RoomManager room, ResultPopUp result, DayandNight dayandNight)
    {
        if (roomManager == null)
        {
            roomManager = room;
            dayAndNight = dayandNight;
            resultPopUp = result;
        }

        DisplayIdx = 0;
        ButtonIdx = 0;
        TutorialTileIdx = 0;

        Cnt = 0;
        StartCoroutine(TakeTask());
    }

    IEnumerator TakeTask()
    {
        TaskManager.isDone = false;
        
        while(Cnt < TaskList.Count)
        {
            print("Cnt" + Cnt);
            isDone = false;
            switch (TaskList[Cnt])
            {
                case TaskType.Arrow: StartCoroutine(Arrow()); break;
                case TaskType.Display: StartCoroutine(Display()); break;
                case TaskType.Fight: StartCoroutine(Fight()); break;
                case TaskType.LongSelect: StartCoroutine(LongSelect()); break;
                case TaskType.MoveMap: StartCoroutine(MoveMap()); break;
                case TaskType.Select: StartCoroutine(Select()); break;
            }
            Cnt++;
            yield return new WaitUntil(IsDone);
        }

        TaskManager.isDone = true;
        TaskManager.taskManager.Dialog.SetActive(true);

        yield break;
    }

    IEnumerator Arrow()
    {
        for (int i = 0; i < 3; i++)
        {
            DisplayObject[DisplayIdx].SetActive(!DisplayObject[DisplayIdx].activeInHierarchy);
            yield return new WaitForSeconds(TaskManager.taskManager.arrowSpeed);
        }

        DisplayIdx++;

        yield return new WaitForSeconds(TaskManager.taskManager.clickTime);
        isDone = true;
        yield break;
    }

    public void SetSlotStatus(bool isPossible) //설치를 위한 슬롯 세팅
    {
        ClickSlot clickSlot = ButtonList[ButtonIdx].GetComponent<ClickSlot>();

        ButtonList[ButtonIdx].GetComponent<Button>().enabled = !isPossible;
        clickSlot.isDisplay = isPossible;

        if (isPossible)
        {
            clickSlot.taskObject = this;

            ButtonParent = ButtonList[ButtonIdx].parent;
            ButtonSiblingIdx = ButtonList[ButtonIdx].GetSiblingIndex();

            ButtonList[ButtonIdx].SetParent(TaskManager.taskManager.Mask);
        }
        else
        {
            clickSlot.taskObject = null;

            ButtonList[ButtonIdx].SetParent(ButtonParent);
            ButtonList[ButtonIdx].SetSiblingIndex(ButtonSiblingIdx);
        }
    }

    public void SetWarpExit()
    {
        if (TutorialTile[TutorialTileIdx].GetComponent<TutorialTile>().objName.Equals("Warp_Exit"))
        {
            InitDisplay(false);
            SetButton(false);

            TutorialTile[0].GetComponent<TutorialTile>().lastColList.Clear();

            Cnt = 1;
            DisplayIdx = 1;
            ButtonIdx = 1;
            TutorialTileIdx = 0;

            StopAllCoroutines();
            print("All Stop + Start");
            StartCoroutine(TakeTask());
        }
    }

    public void StartDisplay() //워프 엑싯의 경우에는 ClickSlot이 아니라 MouseDown으로 처리
    {
        TaskManager.taskManager.OnOffSign(TaskManager.Type.Slide, false);
        
        DisplayObject[DisplayIdx].SetActive(false);
        isClick = true;
        SetSlotStatus(false);
    }

    public void OnDisplay(bool isDisplay) //워프 엑싯의 경우에는 CreateObject가 아니라 MouseUp으로 처리
    {
        this.isDisplay = isDisplay;
    }

    bool IsDisplay()
    {
        return isDisplay;
    }

    bool IsDisplayWarpExit()
    {
        if (Input.GetMouseButtonUp(0))
        {
            return TutorialTile[TutorialTileIdx].GetComponent<TutorialTile>().isSuccess();
        }
        else
            return false;
    }

    void InitDisplay(bool isStart)
    {
        TaskManager.taskManager.isMove = isStart;
        RoomManager.possibleDrag = isStart;

        TutorialTile[TutorialTileIdx].SetActive(isStart);
        TaskManager.taskManager.createPopUp.isTutorial = isStart;

        if (isStart)
        {
            TaskManager.taskManager.createPopUp.tutorialTile = TutorialTile[TutorialTileIdx].GetComponent<TutorialTile>();
            TaskManager.taskManager.createPopUp.taskObject = this;

            TaskManager.taskManager.createObject.tutorialTile = TutorialTile[TutorialTileIdx].GetComponent<TutorialTile>();
            TaskManager.taskManager.createObject.taskObject = this;

            TaskManager.taskManager.createPopUp.transform.SetParent(TaskManager.taskManager.Mask);
        }
        else
        {
            TaskManager.taskManager.createPopUp.tutorialTile = null;
            TaskManager.taskManager.createPopUp.taskObject = null;

            TaskManager.taskManager.createObject.tutorialTile = null;
            TaskManager.taskManager.createObject.taskObject = null;

            TaskManager.taskManager.createPopUp.transform.SetParent(TaskManager.taskManager.CreatePopUpParent);
        }
    }

    bool IsMouseDown()
    {
        return Input.GetMouseButtonDown(0) || Input.GetMouseButton(0);
    }

    IEnumerator MouseDown()
    {
        yield return new WaitUntil(IsMouseDown);
        StartDisplay();
        yield break;
    }



    IEnumerator Display()
    {
        isDisplay = false;
        isClick = false;
        InitDisplay(true);

        bool isWarpExit = TutorialTile[TutorialTileIdx].GetComponent<TutorialTile>().objName.Equals("Warp_Exit");
        if (isWarpExit)
            RoomManager.possibleDrag = false;

        TaskManager.taskManager.OnOffSign(TaskManager.Type.Slide, true);
        SetSlotStatus(true);

        if (!isWarpExit)
        {
            while (!isClick)
            {
                DisplayObject[DisplayIdx].SetActive(!DisplayObject[DisplayIdx].activeInHierarchy);
                yield return new WaitForSeconds(TaskManager.taskManager.loopArrowSpeed);
            }
        }
        else
        {
            StartCoroutine(MouseDown());
            while (!isClick)
            {
                DisplayObject[DisplayIdx].SetActive(!DisplayObject[DisplayIdx].activeInHierarchy);
                yield return new WaitForSeconds(TaskManager.taskManager.loopArrowSpeed);
            }
        }

        if (!isWarpExit)
            yield return new WaitUntil(IsDisplay);
        else
            yield return new WaitUntil(IsDisplayWarpExit);

        DisplayIdx++;

        InitDisplay(false);
        SetSlotStatus(false);
        TutorialTileIdx++;
        ButtonIdx++;

        isDone = true;
        yield break;
    }

    public void OnResult()
    {
        isResult = true;
    }

    bool IsResult()
    {
        return isResult;
    }

    IEnumerator Fight()
    {
        isResult = false;
        resultPopUp.taskObject = this;

        //yield return new WaitForSeconds(2f);
        //dayAndNight.changeState();

        yield return new WaitUntil(IsResult);
        resultPopUp.taskObject = null;
        isDone = true;
        yield break;
    }

    bool IsLongClick()
    {
        return isLongClick;
    }

    public void OnLongSelect()
    {
        isLongClick = true;
        TaskManager.taskManager.OnOffSign(TaskManager.Type.LongSelect, false);
        DisplayObject[DisplayIdx].SetActive(false);
    }

    IEnumerator LongSelect()
    {
        isLongClick = false;
        TaskManager.taskManager.OnOffSign(TaskManager.Type.LongSelect, true);

        ClickObject.taskObject = this;
        ClickObject.isPossibleClick = true;
        TaskManager.taskManager.isPossibleClick = true;


        while (!isLongClick)
        {
            DisplayObject[DisplayIdx].SetActive(!DisplayObject[DisplayIdx].activeInHierarchy);
            yield return new WaitForSeconds(TaskManager.taskManager.loopArrowSpeed);
        }

        DisplayIdx++;

        ClickObject.taskObject = null;
        ClickObject.isPossibleClick = false;
        TaskManager.taskManager.isPossibleClick = false;

        isDone = true;
        yield break;
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

    bool isMoveMap()
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

    IEnumerator MoveMap()
    {
        TaskManager.taskManager.OnOffSign(TaskManager.Type.Slide, true);

        for (int i = 0; i < 3; i++)
        {
            DisplayObject[DisplayIdx].SetActive(!DisplayObject[DisplayIdx].activeInHierarchy);
            yield return new WaitForSeconds(TaskManager.taskManager.arrowSpeed);
        }

        roomManager.prePos = GetRay();
        roomManager.conPos = GetRay();

        yield return new WaitUntil(isMoveMap);

        TaskManager.taskManager.OnOffSign(TaskManager.Type.Slide, false);
        DisplayObject[DisplayIdx].SetActive(false);
        DisplayIdx++;

        isDone = true;
        yield break;
    }

    void SetButton(bool isPossible)
    {
        SlotManager slotManager = ButtonList[ButtonIdx].GetComponent<SlotManager>();

        if (isPossible)
        {
            ButtonParent = ButtonList[ButtonIdx].parent;
            ButtonSiblingIdx = ButtonList[ButtonIdx].GetSiblingIndex();
            if (slotManager != null)
            {
                slotManager.taskObject = this;
                slotManager.isClick = true;
            }
            else
                ButtonList[ButtonIdx].GetComponent<Button>().onClick.AddListener(OnButtonClick);

            ButtonList[ButtonIdx].SetParent(TaskManager.taskManager.Mask);
        }
        else
        {
            if (slotManager != null)
            {
                slotManager.taskObject = null;
                slotManager.isClick = false;
            }
            else
                ButtonList[ButtonIdx].GetComponent<Button>().onClick.RemoveListener(OnButtonClick);

            ButtonList[ButtonIdx].SetParent(ButtonParent);
            ButtonList[ButtonIdx].SetSiblingIndex(ButtonSiblingIdx);

            ButtonIdx++;
        }
    }

    bool IsClick()
    {
        return isClick;
    }

    public void OnButtonClick()
    {
        isClick = true;
        TaskManager.taskManager.OnOffSign(TaskManager.Type.Select, false);
        DisplayObject[DisplayIdx].SetActive(false);

        SetButton(false);
    }

    IEnumerator Select()
    {
        isClick = false;

        if (ButtonList.Count == 0)
            ButtonList.Insert(0, TaskManager.taskManager.OriginalMap.transform.GetChild(2));
        else if (ButtonList[ButtonIdx].name.Equals("MiniMap"))
            yield return new WaitForSeconds(2.3f);
        

        SetButton(true);
        TaskManager.taskManager.OnOffSign(TaskManager.Type.Select, true);

        while(!isClick)
        {
            DisplayObject[DisplayIdx].SetActive(!DisplayObject[DisplayIdx].activeInHierarchy);
            yield return new WaitForSeconds(TaskManager.taskManager.loopArrowSpeed);
        }

        DisplayIdx++;

        isDone = true;
        yield break;
    }
}
