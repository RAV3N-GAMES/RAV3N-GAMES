using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.IO;


public class TileManager : MonoBehaviour {
    const int TILE_MAX = 20;
    int[][] tileMatrix;            //사용하지 않을 경우 -1 , 사용할 경우 type으로 한다.

    List<TileObject> objectList;
    List<SaveObject> saveObj;

    GameObject ChangePopUp;


    void Awake()
    {
        tileMatrix = new int[TILE_MAX][];

        for(int i = 0; i< tileMatrix.Length; i++)
        {
            tileMatrix[i] = new int[TILE_MAX];
        }

        objectList = new List<TileObject>();
        saveObj = new List<SaveObject>();

        ChangePopUp = FindObjectOfType<CreateObject>().ChangePopUp;

        InitMatrix();

        LoadTileObject();
    }


    void InitMatrix()
    {
        for(int i = 0; i < TILE_MAX; i++)
        {
            for(int j = 0; j <TILE_MAX; j++)
            {
                tileMatrix[i][j] = -1;
            }
        }
    }

    public bool isBuilding(int[] idx)
    {
        try
        {
            for (int i = 0; i < idx.Length; i = i + 2)
            {
                if (tileMatrix[idx[i]][idx[i + 1]] != 0)
                    return false;
            }
        }
        catch { return false; }

        return true;
    }

    public bool isEnableTile(int[] idx)
    {
        for(int i = 0; i < idx.Length; i = i + 2)
        {
            try
            {
                if (tileMatrix[idx[i]][idx[i + 1]] >= 0)
                    return false;
                else if (idx[i] >= TILE_MAX || idx[i + 1] >= TILE_MAX || idx[i] < 0 || idx[i + 1] < 0)
                    return false;

            }
            catch (System.IndexOutOfRangeException) { return false; }
        }

        return true;
    }

    public void DestroyObj(int layerDepth, int[] idx)
    {
        for (int i = 0; i < idx.Length; i = i + 2)
        {
            tileMatrix[idx[i]][idx[i + 1]] = -1;
            //print("DestroyIdx" + i + "," + i + 1 + " : " + idx[i] + "," + idx[i + 1]);
        }
        
        objectList.RemoveAt(layerDepth);

        for(int i = layerDepth; i < objectList.Count; i++)
        {
            objectList[i].SetLayerDepth(i);
        }

        for (int i = 0; i < saveObj.Count; i++)
        {
            //print("SaveObj" + " : " + saveObj[i].mRow + "," + saveObj[i].mCol);
            if (saveObj[i].mRow == idx[0])
            {
                if (saveObj[i].mCol == idx[1])
                {
                    saveObj.RemoveAt(i);
                    //print("remove save obj");
                    return;
                }
            }
        }
    }

    void SetOrderInLayer(TileObject newObj)
    {
        int Size = objectList.Count;
        objectList.Add(newObj);

        if (objectList.Count == 1)
        {
            SetOrderInLayer();
            return;
        }

        for (int i = 1; i < objectList.Count; i++)
        {
            TileObject tempTile = objectList[Size - i];

            //if (tempTile.mRow < newObj.mRow)
            //{
            //    break;
            //}
            //else if (tempTile.mRow == newObj.mRow)
            //{
            //    if (tempTile.mCol < newObj.mCol)
            //        break;
            //}


            //그냥 모든 인덱스 값에 대하여 해야할것같은 느낌적인 느낌... 매력적인느낌..

            if (tempTile.mRow < newObj.mRow)
            {
                if(tempTile.mCol <= newObj.mCol)
                    break;
            }
            else if(tempTile.mRow == newObj.mRow)
            {
                if (tempTile.mCol < newObj.mCol)
                    break;
            }
            //else if( tempTile.mRow > newObj.mRow)
            //{
            //    if (tempTile.mCol == newObj.mCol)
            //        break;
            //}
            
            objectList[Size - i] = objectList[Size - i + 1];

            objectList[Size - i + 1] = tempTile;
        }

        SetOrderInLayer();
    }

    void SetOrderInLayer()
    {
        for(int i = 0; i < objectList.Count; i++)
        {
            objectList[i].SetOrderInLayer(name, (objectList.Count - i) * 2, i);
        }
    }

    public void UsingTile(GameObject Obj, int[] idx) //addSave안해도 될듯 0 Obj에 값 제대로 넣어서 만들어야함 애초에
    {
        ObjectInfo objInfo = Obj.GetComponent<ObjectInfo>();

        for (int i = 0; i < idx.Length; i = i + 2)
        {
            tileMatrix[idx[i]][idx[i + 1]] = objInfo.type;
        }


        //여기에서 정렬하면서 추가 -> Layer 변경
        SetOrderInLayer(new TileObject(Obj, idx[0], idx[1]));

        saveObj.Add(new SaveObject(Obj.transform.position, objInfo.type, objInfo.id, objInfo.level,
                                   objInfo.presentHP, objInfo.totalHP, idx[0], idx[1],
                                   objInfo.coordinate, objInfo.pivotObject.name, objInfo.isRotation));

    }

    public void OnTransparency(bool isTransparency)
    {
        if (objectList == null)
            return;

        for (int i = 0; i < objectList.Count; i++)
            objectList[i].OnTransparency(isTransparency);
    }

    GameObject InitObj(SaveObject objInfo, Vector3 pos)
    {//함수 세분화해서 정리
        GameObject newObj = Instantiate(Resources.Load("Object/" + objInfo.id) as GameObject);
        newObj.name = objInfo.id;
        newObj.GetComponent<ClickObject>().ChangePopUp = ChangePopUp;
        newObj.transform.position = pos;

        newObj.GetComponent<CheckTile>().tileManager = this;

        ObjectInfo newObjInfo = newObj.GetComponent<ObjectInfo>();
        newObjInfo.isDisplay = true;
        newObjInfo.InitObject(objInfo);

        return newObj;
    }

    Vector3 GetVector(string pos)
    {
        string[] strPos = pos.Split('/');
        Vector3 posVector = new Vector3(float.Parse(strPos[0]), float.Parse(strPos[1]), float.Parse(strPos[2]));

        return posVector;
    }



    SaveObject GetSaveObject(JsonData data)
    {
        int type = int.Parse(data["type"].ToString());
        string id = data["id"].ToString();
        int level = int.Parse(data["level"].ToString());
        int presentHP = int.Parse(data["presentHP"].ToString());
        int totalHP = int.Parse(data["totalHP"].ToString());
        int row = int.Parse(data["mRow"].ToString());
        int col = int.Parse(data["mCol"].ToString());
        int[] coordinate = new int[data["coordinate"].Count];
        string pivotObject = data["pivotObject"].ToString();

        bool isRotation = bool.Parse(data["isRotation"].ToString());

        for (int j = 0; j < coordinate.Length; j++)
        {
            coordinate[j] = int.Parse(data["coordinate"][j].ToString());
        }
        
        return new SaveObject(GetVector(data["pos"].ToString()), type, id, level, presentHP, totalHP, row, col, coordinate, pivotObject, isRotation);
    }

    int[] makeIdx(int mRow, int mCol, int[] coordinate)
    {
        int[] idx = new int[coordinate.Length];

        idx[0] = mRow;
        idx[1] = mCol;

        for (int i = 0; i < idx.Length; i = i + 2)
        {
            idx[i] = idx[0] + coordinate[i];
            idx[i + 1] = idx[1] + coordinate[i + 1];
        }

        return idx;
    }

    void LoadTileObject()
    {
        try
        {
            string jsonObj = File.ReadAllText(Application.dataPath + "/Resources/Data/Room" + gameObject.name + ".json");
            JsonData displayObj = JsonMapper.ToObject(jsonObj);
            
            for (int i = 0; i < displayObj.Count; i++)
            {
                SaveObject obj = GetSaveObject(displayObj[i]);

                UsingTile(InitObj(obj, GetVector(obj.pos)), makeIdx(obj.mRow, obj.mCol, obj.coordinate));
            }
        }
        catch { }
    }

    void SaveTileObject()
    {
        JsonData newObj = JsonMapper.ToJson(saveObj);
        File.WriteAllText(Application.dataPath + "/Resources/Data/Room" + gameObject.name + ".json", newObj.ToString());
    }

    void OnApplicationQuit()
    {
        SaveTileObject();
    }
}
