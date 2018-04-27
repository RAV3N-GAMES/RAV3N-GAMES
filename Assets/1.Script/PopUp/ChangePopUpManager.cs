using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangePopUpManager : MonoBehaviour {
    [Tooltip("0 : Building, 1 : OurForces, 2 : Trap, 3 : Secret")]
    public List<GameObject> changePopUpObj;
    List<ChangePopUp> changePopUp;

    int idx;

    void Awake()
    {
        changePopUp = new List<ChangePopUp>();

        for (int i = 0; i < changePopUpObj.Count; i++)
            changePopUp.Add(changePopUpObj[i].GetComponent<ChangePopUp>());
    }

	public void InitChangePopUp(GameObject Obj, int type)
    {
        for (int i = 0; i < changePopUpObj.Count; i++)
            changePopUpObj[i].SetActive(false);

        switch (type)
        {
            case 0: idx = 0; break;
            case 2: idx = 1; break;
            case 3: idx = 2; break;
            case 4: idx = 3; break;
        }

        changePopUpObj[idx].SetActive(true);
        changePopUp[idx].Obj = Obj;
        changePopUp[idx].InitPopUp();
    }

    public void DestroyPref()
    {
        changePopUp[idx].DestroyPref();
    }

    public void RepairPref()
    {
        changePopUp[idx].RepairPref();
    }

    public void PartialRepairPref()
    {
        changePopUp[idx].PartialRepairPref();
    }
}
