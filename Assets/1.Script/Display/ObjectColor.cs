using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectColor : MonoBehaviour { //수정완료
    public SpriteRenderer sprite;
    Color mColor;

    public Color SuccessColor;
    public Color FailColor;

    [HideInInspector]
    public bool isPossible;
    TileManager tileManager; //지금은 방이 하나이므로
    
    [HideInInspector]
    public List<GameObject> lastCol;

    // Use this for initialization
    void Start()
    {
        mColor = sprite.color;
        lastCol = new List<GameObject>();

        tileManager = FindObjectOfType<TileManager>();
    }

    public void OnColor(bool possible)
    {
        isPossible = possible;
        sprite.gameObject.SetActive(true);
        if (possible)
            sprite.color = SuccessColor;
        else
            sprite.color = FailColor;
    }

    public void OffColor()
    {
        isPossible = false;
        sprite.gameObject.SetActive(false);
        sprite.color = mColor;
    }

    public int[] makeObjIdx()
    {
        int[] idx = new int[lastCol.Count * 2];

        for (int i = 0; i < idx.Length; i = i + 2)
        {
            idx[i] = System.Int32.Parse(lastCol[i / 2].transform.parent.name);
            idx[i + 1] = System.Int32.Parse(lastCol[i / 2].name);
        }

        return idx;
    }

    bool isEnable()
    {
        if (lastCol.Count == 0)
            return false;

        int[] idx = makeObjIdx();

        return tileManager.isEnableTile(idx);
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Tile")
        {
            lastCol.Add(col.gameObject);
            OnColor(isEnable());
        }
    }

    void OnTriggerExit(Collider col) //테스트용 (수정해야함)
    {
        if (col.tag == "Tile")
        {
            lastCol.Remove(col.gameObject);
            OnColor(isEnable());
        }
    }
}
