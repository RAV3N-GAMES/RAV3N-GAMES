using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayObject : MonoBehaviour {
    GameObject CreateButton;
    public GameObject ChangeButton;

    [HideInInspector]
    public Transform pivotObject;
    public Transform lastCol { get; private set; }
    ObjectColor objectColor;

    TileManager tileManager;

    void Awake()
    {
        CreateButton = GameObject.Find("Canvas").transform.Find("Create").gameObject;
        objectColor = GetComponent<ObjectColor>();

        tileManager = FindObjectOfType<TileManager>();
    }

    public void OnDisplay()
    {
        Array();
        OnCheckButton();
    }

    void Array()
    {
        StartCoroutine("MyArray"); //좋지않음
    }

    void findPivotCol()
    {
        int Cnt = objectColor.lastCol.Count;
        GameObject pivotCol = objectColor.lastCol[0];

        for (int i = 1; i < Cnt; i++)
        {
            if(objectColor.lastCol[i].transform.position.y > pivotCol.transform.position.y)
            {
                pivotCol = objectColor.lastCol[i];
            }
        }

        lastCol = pivotCol.transform;
    }

    public void UsingTile()
    {
        int[] coordinate = GetComponent<ObjectInfo>().coordinate;
        int[] idx = new int[coordinate.Length];

        idx[0] = System.Int32.Parse(lastCol.transform.parent.name);
        idx[1] = System.Int32.Parse(lastCol.name);

        for(int i = 0; i < idx.Length; i = i + 2)
        {
            idx[i] = idx[0] + coordinate[i];
            idx[i + 1] = idx[1] + coordinate[i + 1];
        }
        

        for(int i = 0; i < idx.Length; i = i + 2)//보려고
        {
            print(idx[i] + "," + idx[i + 1]);
        }

        tileManager.UsingTile(gameObject, idx);
    }

    void GetPivotObject()
    {
        if (transform.childCount <= 0)
            return;

        Transform pivot = transform.GetChild(0);
        for(int i = 1; i < transform.childCount; i++ )
        {
            if(pivot.position.y > transform.GetChild(i).position.y)
            {
                pivot = transform.GetChild(i);
            }
        }

        pivotObject = pivot;
    }

    IEnumerator MyArray()
    {
        findPivotCol();
        GetPivotObject();
        pivotObject = lastCol.transform;

        print(lastCol.transform.parent.name + "," + lastCol.name);
        yield return null;
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, Camera.main.transform.position.z);
    }

    void OnCheckButton()
    {
        CreateButton.SetActive(true);

        PrefControl prefControl = CreateButton.GetComponent<PrefControl>();
        prefControl.Obj = gameObject;
        prefControl.displayObject = this;
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
