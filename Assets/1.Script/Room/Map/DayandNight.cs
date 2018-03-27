using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum OurForces_Sprite { Guard , QuickReactionForces , BiochemistryUnit , Researcher };
public enum OurForces_Animation { Friendly_Guard, Friendly_Army, Friendly_ChemistryArmy, Friendly_ResearchStudent };
public enum OurForces_Script { FriendlyGuard, FriendlyArmy, FriendlyChemistry , FriendlyResearcher};

public class DayandNight : MonoBehaviour
{
    public static bool isDay;
    public static bool DayTime;
    public CanvasRenderer curtain;
    public const int EnumMax = (int)OurForces_Sprite.Researcher;//얘는 나중에 아군캐릭터 추가되면 바꿔줘야됨
    //DayTime은 추후에 json 파일에서 읽어올 수 있음.

    // Use this for initialization
    void Awake () {
        isDay = false;//밤부터 시작
	}
	
	// Update is called once per frame
	void Update () {
	}

    public void changeState() {
        isDay = !isDay;
        Debug.Log("Day: " + isDay);
        if (isDay) {
            curtain.transform.Rotate(0, 90, 0);
            Debug.Log("Rotate: " + curtain.transform.rotation);
        }
        else {
            curtain.transform.Rotate(0, -90, 0);
            Debug.Log("Rotate: " + curtain.transform.rotation);
        }

        CharacterEnable(isDay);
    }

    IEnumerator DayCount(float a) {
        yield return new WaitForSeconds(a);
        changeState();
    }

    public bool CharacterEnable(bool Day) {
        bool CharacterEnabled = false;
        GameObject[] OurForces = GameObject.FindGameObjectsWithTag("Friendly");
        GameObject spriteObject=null;
        GameObject animationObject = null;
       for (int i = 0; i < OurForces.Length; i++) {
            try
            {
                getObjects(OurForces[i], ref spriteObject, ref animationObject);
                if (Day)
                {
                    if (spriteObject != null)
                    {
                        spriteObject.SetActive(false);
                        setOriginalPoint(ref animationObject, ref spriteObject);
                        animationObject.SetActive(true);
                        CharacterEnabled = true;
                    }
                }
                else
                {
                    if (spriteObject != null)
                    {
                        spriteObject.SetActive(true);
                        animationObject.SetActive(false);
                        CharacterEnabled = false;
                    }
                }
            }
            catch { }
        }
        return CharacterEnabled;
    }

    public void getObjects(GameObject OurForces, ref GameObject spriteObject, ref GameObject animationObject) {
        spriteObject = null;
        animationObject = null;
        for (int i = 0; i <EnumMax;i++) {
            try
            {
                spriteObject = OurForces.transform.FindChild(((OurForces_Sprite)i).ToString()).gameObject;
                animationObject = OurForces.transform.FindChild(((OurForces_Animation)i).ToString()).gameObject;
            }
            catch { }
            if (spriteObject != null)
            {
                break;
            }
        }
    }

    public void setOriginalPoint(ref GameObject animationObject, ref GameObject spriteObject) {
        animationObject.transform.FindChild("Human").GetComponent<Friendly>().OriginalPoint = spriteObject.transform;
    }
}