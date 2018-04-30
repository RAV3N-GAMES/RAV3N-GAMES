using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjInfoPopUpManager : MonoBehaviour {
    [Tooltip("0 : Building, 1 : OurForces, 2 : Trap, 3 : Secret")]
    public List<GameObject> objInfoPopUpObj;
    public List<ObjInfoPopUp> objInfoPopUp;

    int idx;

    public void InitObjInfoPopUp(string id, int type, SlotManager slotManager)
    {
        for (int i = 0; i < objInfoPopUpObj.Count; i++)
            objInfoPopUpObj[i].SetActive(false);

        switch (type)
        {
            case 0: idx = 0; break;
            case 2: idx = 1; break;
            case 3: idx = 2; break;
            case 4: idx = 3; break;
        }
        
        objInfoPopUpObj[idx].SetActive(true);
        objInfoPopUp[idx].InitObjInfoPopUp(id, type, slotManager);
    }

    public void UpgradePref()
    {
        objInfoPopUp[idx].LevelUp();
    }

    public void ActivationPref()
    {
        objInfoPopUp[idx].OnActivation();
    }
}
