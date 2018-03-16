using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickSlot : MonoBehaviour, IPointerDownHandler {
    [HideInInspector]
    public string id;
    [HideInInspector]
    public CreateObject createObject;

    public void OnPointerDown(PointerEventData data)
    {
        createObject.id = id;
    }
}
