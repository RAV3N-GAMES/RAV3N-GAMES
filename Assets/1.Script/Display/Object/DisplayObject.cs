using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayObject : MonoBehaviour
{
    public bool possibleRotation;

    CheckTile checkTile;
    public GameObject CreateButton;

    Transform lastCol;

    void Awake()
    {
        checkTile = GetComponent<CheckTile>();
    }

    public void OnDisplay()
    {
        Array();

        checkTile.OnDisplayCheckTile();
        OnCheckButton();
    }

    void Array()
    {
        lastCol = checkTile.findPivotCol();

        Camera.main.transform.position = new Vector3(transform.position.x, Camera.main.transform.position.y, transform.position.z);
    }

    int[] makeIdx()
    {
        if(lastCol == null)
            lastCol = checkTile.findPivotCol();

        int[] coordinate = GetComponent<ObjectInfo>().coordinate;
        int[] idx = new int[coordinate.Length];

        idx[0] = System.Int32.Parse(lastCol.transform.parent.name);
        idx[1] = System.Int32.Parse(lastCol.name);

        for (int i = 0; i < idx.Length; i = i + 2)
        {
            idx[i] = idx[0] + coordinate[i];
            idx[i + 1] = idx[1] + coordinate[i + 1];
        }

        return idx;
    }


    public bool DestroyObj(bool isDestroyed)
    {
        return checkTile.DestroyObj(isDestroyed, makeIdx());
    }

    public bool UsingTile()
    {
        return checkTile.UsingTile(makeIdx());
    }

    void OnCheckButton()
    {
        CreateButton.SetActive(true);

        CreatePopUp createPopUp = CreateButton.GetComponent<CreatePopUp>();
        if (createPopUp != null)
        {
            createPopUp.Obj = gameObject;
            createPopUp.RotationButton.SetActive(possibleRotation);
        }
        else
        {
            MovePopUp movePopUp = CreateButton.GetComponent<MovePopUp>();
            movePopUp.RotationButton.SetActive(possibleRotation);
        }
    }

    void OffCheckButton()
    {
        CreateButton.SetActive(false);
    }

    public void OffDisplay()
    {
        OffCheckButton();
    }
}
