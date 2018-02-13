using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefControl : MonoBehaviour {
    [HideInInspector]
    public GameObject Obj;
    [HideInInspector]
    public DisplayObject displayObject;

	public void CreatePref()
    {//OnMouseDown 이벤트가 먼저들어가서 해당 함수 호출 안하는 경우가 있음
        GameObject prefIns = Instantiate(Resources.Load("Temp/" + Obj.name) as GameObject); // 이거 변경해야함

        prefIns.transform.position = Obj.transform.position;
        prefIns.name = Obj.name;

        displayObject.UsingTile();

        gameObject.SetActive(false);
        Destroy(Obj);
    }

    public void DestroyPref()
    {
        Destroy(Obj);
        gameObject.SetActive(false);
    }

    public void MovePref()
    {
        print("MovePref");
    }

    public void CancelPref()
    {
        Destroy(Obj);
        gameObject.SetActive(false);
    }
}
