using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.IO;


public class TileManager : MonoBehaviour {
    const int TILE_MAX = 20;
    bool[][] tileMatrix;

    List<TileObject> objectList;
    List<SaveObject> saveObj;

    GameObject ChangePopUp;

    void Awake()
    {
        tileMatrix = new bool[TILE_MAX][];

        for(int i = 0; i< tileMatrix.Length; i++)
        {
            tileMatrix[i] = new bool[TILE_MAX];
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
            else if (idx[i] >= TILE_MAX || idx[i + 1] >= TILE_MAX || idx[i] < 0 || idx[i + 1] < 0)
                return false;
        }

        return true;
    }

    public void DestroyObj(GameObject Obj, int[] idx)
    {
        for (int i = 0; i < idx.Length; i = i + 2)
        {
            tileMatrix[idx[i]][idx[i + 1]] = true;
        }

        for (int i = 0; i < objectList.Count; i++)
        {
            if (objectList[i].mObject == Obj)
            {
                objectList.RemoveAt(i);
                saveObj.RemoveAt(i);
                break;
            }
        }
    }

    public void UsingTile(GameObject Obj, int[] idx) //addSave안해도 될듯 0 Obj에 값 제대로 넣어서 만들어야함 애초에
    {
        for (int i = 0; i < idx.Length; i = i + 2)
        {
            tileMatrix[idx[i]][idx[i + 1]] = false;
        }

        //여기에서 정렬하면서 추가 -> Layer 변경
        objectList.Add(new TileObject(Obj, idx[0], idx[1]));
        ObjectInfo objInfo = Obj.GetComponent<ObjectInfo>();


        saveObj.Add(new SaveObject(Obj.transform.position, objInfo.id, objInfo.level,
                                   objInfo.presentHP, objInfo.totalHP, idx[0], idx[1],
                                   objInfo.coordinate, objInfo.pivotObject.name));
    }

    int[] UsingLoadTile(int[] coordinate,int row, int col) //없애야함//ObjectInfo 스크립트 수정
    {
        int[] idx = new int[coordinate.Length];

        idx[0] = row;
        idx[1] = col;

        for (int i = 0; i < idx.Length; i = i + 2)
        {
            idx[i] = idx[0] + coordinate[i];
            idx[i + 1] = idx[1] + coordinate[i + 1];
        }

        return idx;
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
        string id = data["id"].ToString();
        int level = int.Parse(data["level"].ToString());
        int presentHP = int.Parse(data["presentHP"].ToString());
        int totalHP = int.Parse(data["totalHP"].ToString());
        int row = int.Parse(data["mRow"].ToString());
        int col = int.Parse(data["mCol"].ToString());
        int[] coordinate = new int[data["coordinate"].Count];
        string pivotObject = data["pivotObject"].ToString();

        for (int j = 0; j < coordinate.Length; j++)
        {
            coordinate[j] = int.Parse(data["coordinate"][j].ToString());
        }
        
        return new SaveObject(GetVector(data["pos"].ToString()), id, level, presentHP, totalHP, row, col, coordinate, pivotObject);
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


                int[] tileIdx = UsingLoadTile(obj.coordinate, obj.mCol, obj.mRow);

                UsingTile(InitObj(obj, GetVector(obj.pos)), tileIdx);
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
