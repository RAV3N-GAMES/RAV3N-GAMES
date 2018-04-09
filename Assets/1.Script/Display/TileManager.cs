using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.IO;

public class TileManager : MonoBehaviour {
    const int TILE_MAX = 20;
    float[][] tileMatrix;          //사용하지 않을 경우 -1 , 사용할 경우 type으로 한다.

    List<TileObject> objectList;
    List<SaveObject> saveObj;

    GameObject ChangePopUp;
    GameObject CreatePopUp;

    MapManager mapManager;

    int warpCol;
    int warpRow;


    public List<SaveObject> GetOurForcesInfo()
    {
        List<SaveObject> ourForcesList = new List<SaveObject>();

        for(int i = 0; i < saveObj.Count; i++)
        {
            if (saveObj[i].type == 2)
                ourForcesList.Add(saveObj[i]); //깊은 복사를 해야하는가..
        }
        
        return ourForcesList;
    }


    void Awake()
    {
        tileMatrix = new float[TILE_MAX][];

        for(int i = 0; i< tileMatrix.Length; i++)
        {
            tileMatrix[i] = new float[TILE_MAX];
        }

        objectList = new List<TileObject>();
        saveObj = new List<SaveObject>();

        CreateObject createObject = FindObjectOfType<CreateObject>();
        ChangePopUp = createObject.ChangePopUp;
        CreatePopUp = createObject.CreatePopUp;

        mapManager = FindObjectOfType<MapManager>();

        warpRow = -1;
        warpCol = -1;

        InitMatrix();

        LoadTileObject();
    }

    public void SetMatrix(int[] idx, float type)
    {
        for(int i = 0; i < idx.Length; i = i + 2)
        {
            tileMatrix[idx[i]][idx[i + 1]] = type;
        }
    }

    public int GetObjectCount()
    {
        if (objectList == null)
            return 0;
        return objectList.Count;
    }

    public float[] GetObjectInfo(int idx) //0 : row, 1 : col, 2 : type 
    {
        float[] info = new float[3];

        float[] center = objectList[idx].CenterCoordinate();

        info[0] = center[0];
        info[1] = center[1];

        info[2] = tileMatrix[objectList[idx].mRow][objectList[idx].mCol];

        return info;
    }

    void InitMatrix()
    {
        int initNum = -1;
        if (name.Equals("0"))
            initNum = 5;

        for(int i = 0; i < TILE_MAX; i++)
        {
            for(int j = 0; j <TILE_MAX; j++)
            {
                tileMatrix[i][j] = initNum;
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

    public int isFlameThrowingTrap(int[] idx)
    {
        int Cnt = 0;
        for (int i = 0; i < idx.Length; i = i + 2)
        {
            try
            {
                if (tileMatrix[idx[i]][idx[i + 1]] == 3.5f)
                {
                    Cnt += 1;
                }
            }
            catch (System.IndexOutOfRangeException) {}
        }

        return Cnt;
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

    void OffDependency(GameObject Obj, int[] pivotCoordinate)
    {
        ObjectInfo objInfo = Obj.GetComponent<ObjectInfo>();

        if (objInfo.id == "ObstructMovementCurrent")
        {
            int[] omcIdx = Obj.GetComponent<CheckTile>().DepencyTile_OMC(pivotCoordinate);

            OffDepency(omcIdx[0], omcIdx[1]);
            OffDepency(omcIdx[2], omcIdx[3]);
        }
        else if (objInfo.id == "FlameThrowingTrap")
        {
            int[] fttIdx = Obj.GetComponent<CheckTile>().DepencyTile_FTT(pivotCoordinate);

            for(int i = 0; i < fttIdx.Length; i = i + 2)
            {
                OffDepency(fttIdx[i], fttIdx[i + 1]);
            }
        }
    }
    public bool DestroyObj(bool isDestroyed, GameObject Obj, int[] idx)
    {
        ObjectInfo objInfo = Obj.GetComponent<ObjectInfo>();
        int layerDepth = objInfo.layerDepth;

        if (objInfo.DontDestroy != 0)
            return false;

        int[] pivotCoordinte = { idx[0], idx[1] };
        OffDependency(Obj, pivotCoordinte);

        for (int i = 0; i < idx.Length; i = i + 2)
        {
            tileMatrix[idx[i]][idx[i + 1]] = -1;
        }
        
        objectList.RemoveAt(layerDepth);

        for(int i = layerDepth; i < objectList.Count; i++)
        {
            objectList[i].SetLayerDepth(i);
        }

        for (int i = 0; i < saveObj.Count; i++)
        {
            if (saveObj[i].mRow == idx[0])
            {
                if (saveObj[i].mCol == idx[1])
                {
                    saveObj.RemoveAt(i);
                    break;
                }
            }
        }

        if (isDestroyed)
            DamageReportPopUp.PlusDamage(objInfo.type, objInfo.id);
        mapManager.SetObjectCnt(objInfo.type, -1);

        return true;
    }

    bool ComparePosition(int[] tempIdx, int[] newIdx)
    {
        for(int i = 0; i < tempIdx.Length; i = i + 2)
        {
            for (int j = 0; j < newIdx.Length; j = j + 2)
            {
                if (tempIdx[i + 1] < newIdx[j + 1] && tempIdx[i] < newIdx[j])
                {
                    return true;
                }
                else if(tempIdx[i + 1] < newIdx[j + 1])
                {
                    if (tempIdx[i] == newIdx[j])
                        return true;
                }
                else if (tempIdx[i ] < newIdx[j])
                {
                    if (tempIdx[i + 1] == newIdx[j + 1])
                        return true;
                }
            }
        }

        return false;
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

        int[] newIdx = makeIdx(newObj.mRow, newObj.mCol, newObj.mObject.GetComponent<ObjectInfo>().coordinate);
        for (int i = 1; i < objectList.Count; i++)
        {
            TileObject tempTile = objectList[Size - i];

            if (ComparePosition(makeIdx(tempTile.mRow, tempTile.mCol, tempTile.mObject.GetComponent<ObjectInfo>().coordinate), newIdx))
                break;

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

    void OffDepency(int row, int col)
    {
        int i = 0;
        for (; i < objectList.Count; i++)
        {
            if (objectList[i].isTileBuilding(row, col))
            {
                objectList[i].OffDepency();
                break;
            }
        }

        for (int j = 0; j < saveObj.Count; j++)
        {
            if (saveObj[j].mRow == objectList[i].mRow)
            {
                if (saveObj[j].mCol == objectList[i].mCol)
                {
                    saveObj[j].DontDestroy--;
                    break;
                }
            }
        }
    }

    public void OnDependency(int row, int col)
    {
        int i = 0;

        for (; i < objectList.Count; i++)
        {
            if(objectList[i].isTileBuilding(row, col))
            {
                objectList[i].OnDependency();
                break;
            }
        }

        for (int j = 0; j < saveObj.Count; j++)
        {
            if (saveObj[j].mRow == objectList[i].mRow)
            {
                if (saveObj[j].mCol == objectList[i].mCol)
                {
                    saveObj[j].DontDestroy++;
                    break;
                }
            }
        }
    }

    public void UsingTile(GameObject Obj, int[] idx) //addSave안해도 될듯 0 Obj에 값 제대로 넣어서 만들어야함 애초에
    {
        ObjectInfo objInfo = Obj.GetComponent<ObjectInfo>();

        float type = objInfo.type;
        if (objInfo.id.Equals("FlameThrowingTrap"))
            type += 0.5f;
        for (int i = 0; i < idx.Length; i = i + 2)
        {
            tileMatrix[idx[i]][idx[i + 1]] = type;
        }

        //여기에서 정렬하면서 추가 -> Layer 변경
        SetOrderInLayer(new TileObject(Obj, idx[0], idx[1]));
        
        if(objInfo.id.Equals("Warp"))
        {
            warpRow = idx[0];
            warpCol = idx[1];
        }
        else if(!objInfo.id.Equals("Warp_Exit"))
        {
            warpRow = -1;
            warpCol = -1;
        }

        saveObj.Add(new SaveObject(Obj.transform.position, objInfo.DontDestroy, type, objInfo.id, objInfo.level,
                                   objInfo.presentHP, objInfo.totalHP, idx[0], idx[1],
                                   objInfo.coordinate, objInfo.pivotObject.name, objInfo.isRotation, warpRow, warpCol));

        mapManager.SetObjectCnt(objInfo.type, 1);
    }
    

    public void OnTransparency(bool isTransparency)
    {
        if (objectList == null)
            return;

        for (int i = 0; i < objectList.Count; i++)
        {
            objectList[i].OnTransparency(isTransparency);
        }
    }

    public void SetClickColliderStatus(bool ClickStatus)
    {
        if (objectList == null)
            return;

        for (int i = 0; i < objectList.Count; i++)
        {
            objectList[i].SetClickColliderStatus(ClickStatus);
        }
    }

    GameObject GetParentWarp(int row, int col)
    {
        for(int i = 0; i < objectList.Count; i++)
        {
            if(objectList[i].mRow == row)
            {
                if (objectList[i].mCol == col)
                    return objectList[i].mObject;
            }
        }

        return null;
    }

    GameObject InitObj(SaveObject objInfo, Vector3 pos)
    {   //함수 세분화해서 정리
        GameObject newObj;

        if (objInfo.id.Equals("Warp_Exit"))
        {
            newObj = GetParentWarp(objInfo.parentRow, objInfo.parentCol).transform.Find("Warp_Exit").gameObject;
            newObj.SetActive(true);
        }
        else
            newObj = Instantiate(Resources.Load("Object/" + objInfo.id) as GameObject);

        newObj.name = objInfo.id;
        newObj.transform.position = pos;

        newObj.GetComponent<ClickObject>().ChangePopUp = ChangePopUp;
        newObj.GetComponent<DisplayObject>().CreateButton = CreatePopUp;
        newObj.GetComponent<CheckTile>().tileManager = this;

        ObjectInfo newObjInfo = newObj.GetComponent<ObjectInfo>();
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
        float type = float.Parse(data["type"].ToString());

        int DontDestroy = int.Parse(data["DontDestroy"].ToString());

        string id = data["id"].ToString();
        int level = int.Parse(data["level"].ToString());
        int presentHP = int.Parse(data["presentHP"].ToString());
        int totalHP = int.Parse(data["totalHP"].ToString());
        int row = int.Parse(data["mRow"].ToString());
        int col = int.Parse(data["mCol"].ToString());
        int[] coordinate = new int[data["coordinate"].Count];
        string pivotObject = data["pivotObject"].ToString();

        int isRotation = int.Parse(data["isRotation"].ToString());

        int parentRow = int.Parse(data["parentRow"].ToString());
        int parentCol = int.Parse(data["parentCol"].ToString());

        for (int j = 0; j < coordinate.Length; j++)
        {
            coordinate[j] = int.Parse(data["coordinate"][j].ToString());
        }

        return new SaveObject(GetVector(data["pos"].ToString()), DontDestroy, type, id, level, presentHP, totalHP, row, col, coordinate, pivotObject, isRotation, parentRow, parentCol);
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

                //saveObj 리스트에 순서가 문제가 될수도 있음
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
