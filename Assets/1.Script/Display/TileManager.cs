using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour { //수정완료
    const int TILE_MAX = 20;
    bool[][] tileMatrix;

    List<TileObject> objectInfo;

    void Awake()
    {
        tileMatrix = new bool[TILE_MAX][];

        for(int i = 0; i< tileMatrix.Length; i++)
        {
            tileMatrix[i] = new bool[TILE_MAX];
        }

        InitMatrix(); //여기에서 저장된 정보 불러와야함 //objectInfo도 불러오고
        objectInfo = new List<TileObject>();
    }

    void InitMatrix()
    {
        for(int i = 0; i < TILE_MAX; i++)
        {
            for(int j = 0; j <TILE_MAX; j++)
            {
                tileMatrix[i][j] = true;
            }
        }
    }

    public bool isEnableTile(int[] idx)
    {
        for(int i = 0; i < idx.Length; i = i + 2)
        {
            if (!tileMatrix[idx[i]][idx[i + 1]])
                return false;
        }

        return true;
    }

    public void UsingTile(GameObject Obj, int[] idx)
    {
        for (int i = 0; i < idx.Length; i = i + 2)
        {
            tileMatrix[idx[i]][idx[i + 1]] = false;
        }
        

        //여기에서 정렬하면서 추가 -> Layer 변경
        objectInfo.Add(new TileObject(Obj, idx[0], idx[1]));
    }
}
