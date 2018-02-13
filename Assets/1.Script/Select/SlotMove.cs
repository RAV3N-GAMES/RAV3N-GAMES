using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotMove : MonoBehaviour {
    float preX;
    float postX;

    bool isMove;

    public void BeginMove()
    {
        postX = preX = Input.mousePosition.x;
        isMove = true;
    }

    public void OnMove()
    {
        if (isMove)
        {
            postX = Input.mousePosition.x;
            Vector3 movePos = new Vector3((postX - preX) / 1.5f, 0, 0);

            transform.position += movePos;

            preX = postX;
        }
    }

    public void MouseExit()
    {
        isMove = false;
    }
}
