using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateObject : MonoBehaviour {
    [HideInInspector]
    public string id;

    public GameObject ChangePopUp;
    public GameObject CreatePopUp;

    public RectTransform BoxRect;
    float MaxY;

    bool isCreate;

    void Awake()
    {
        id = "";
        MaxY = BoxRect.anchorMax.y;
        isCreate = false;
    }

    void SetBoxRect(float maxY)
    {
        BoxRect.anchorMax = new Vector2(BoxRect.anchorMax.x, maxY);

        BoxRect.offsetMin = Vector2.zero;
        BoxRect.offsetMax = Vector2.zero;
    }
    

    public void MouseUp()
    {
        id = "";
        SetBoxRect(MaxY);

        if (!isCreate)
            RoomManager.possibleDrag = true;
        isCreate = false;
    }

    public void MouseExit()
    {
        if (id != "" && Input.GetMouseButton(0))
        {
            RoomManager.possibleDrag = false;
            
            GameObject newObj = Instantiate(Resources.Load("Object/" + id) as GameObject);
            newObj.name = id;
            newObj.GetComponent<DisplayObject>().CreateButton = CreatePopUp;
            newObj.GetComponent<ClickObject>().ChangePopUp = ChangePopUp;

            SetBoxRect(BoxRect.anchorMin.y + 0.02f);

            
            isCreate = true;
            id = "";
        }
    }
}
