using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePopUp : MonoBehaviour {
    //타일사용여부 해제 시점 알아야 함
    public GameObject RotationButton;
    public GameObject ExceedLimitAllot;

    GameObject movingObj = null;
    int[] preIdx;
    Vector3 prePos;
    int preRotation;

    TileManager tileManager;
    
    //void SetSortingLayer()
    //{
    //    movingObj.GetComponent<ObjectColor>().SetSortingLayer(tileManager.gameObject.name);
    //}

    public void InitObject(GameObject obj)
    {
        movingObj = obj;
        
        preIdx = movingObj.GetComponent<CheckTile>().makeIdx();
        prePos = movingObj.transform.position;
        preRotation = movingObj.GetComponent<ObjectInfo>().isRotation;

        //처음 시작할 경우
        movingObj.GetComponent<ObjectColor>().SetSortingLayer("NewObject");

        tileManager = movingObj.GetComponent<CheckTile>().tileManager;
        tileManager.SetMatrix(preIdx, -1); //이거 다시 문제임

        //=> 설치가능한 위치로 변경한것
    }
    
    IEnumerator ReturnDisplay()
    {
        yield return null;
        ObjectInfo objectInfo = movingObj.GetComponent<ObjectInfo>();

        while (objectInfo.isRotation != preRotation)
        {
            objectInfo.rotationObject();
        }
        
        movingObj.GetComponent<ObjectInfo>().OnDisplay();
        //SetSortingLayer();  //이거 굳이 해야하나

        if(objectInfo.name.Equals("Warp"))
        {
            ObjectMove objectMove = movingObj.GetComponent<ObjectMove>();
            objectMove.WarpExit.transform.position = objectMove.warpPos;
        }

        gameObject.SetActive(false);

        yield break;
    }
    public void ReturnObj()
    {
        movingObj.transform.position = prePos;

        float type = movingObj.GetComponent<ObjectInfo>().type;
        if (movingObj.name.Equals("FlameThrowingTrap"))
            type += 0.5f;

        tileManager.SetMatrix(preIdx, type);
        RoomManager.ChangeClickStatus(true);

        movingObj.GetComponent<ObjectColor>().SetSortingLayer(tileManager.name);

        StartCoroutine("ReturnDisplay");
    }

    public void MoveObj()
    {
        if (movingObj.GetComponent<CheckTile>().tileManager.name != tileManager.name)
        {
            if (!movingObj.GetComponent<CheckTile>().tileManager.IsPossibleAllot(movingObj.GetComponent<ObjectInfo>().type))
            {
                ReturnObj();
                ExceedLimitAllot.SetActive(true);
                return;
            }
        }

        if (movingObj.GetComponent<DisplayObject>().UsingTile(false))
        {
            TileManager newTileManger = movingObj.GetComponent<CheckTile>().tileManager;

            movingObj.GetComponent<CheckTile>().tileManager = tileManager;
            movingObj.GetComponent<CheckTile>().DestroyObj(false, preIdx);

            movingObj.GetComponent<CheckTile>().tileManager = newTileManger;
            movingObj.GetComponent<DisplayObject>().UsingTile(true);

            movingObj.GetComponent<ObjectInfo>().OnDisplay();
            RoomManager.ChangeClickStatus(true);

            gameObject.SetActive(false);
        }
        else
        {
            ReturnObj();
        }
    }
    
    IEnumerator CheckTile()
    {
        yield return null;
        movingObj.GetComponent<CheckTile>().OnDisplayCheckTile();

        yield break;
    }

    public void RotationObj()
    {
        //코루틴 진행중이면 회전안하게 막아야지
        movingObj.GetComponent<ObjectInfo>().rotationObject();

        StartCoroutine("CheckTile");
    }
}
