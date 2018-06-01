using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum OurForces_Sprite { Guard , QuickReactionForces , BiochemistryUnit , Researcher };
public enum OurForces_Animation { Friendly_Guard, Friendly_Army, Friendly_ChemistryArmy, Friendly_ResearchStudent };
public enum OurForces_Script { FriendlyGuard, FriendlyArmy, FriendlyChemistry , FriendlyResearcher};
public enum Traps_body { Trap_H , Trap_Warp_E , Trap_W };
public enum Traps_Script { HumanTrap , Warp_Enter , Warp_Exit};

public class DayandNight : MonoBehaviour
{
    public EnemyClusterManager EC;
    public RoomManager roomManager;
    public GameObject ReadyButton;
    public GameObject InActReadyButton;
    public Friendly[] FriendlyList;
    public ResultPopUp resultPopUp;

    public static bool isDay = false;
    public static bool DayTime;
    [HideInInspector]
    public static List<Enemy> CreatedEnemy;
    [HideInInspector]
    public static List<Enemy> DeadEnemy;
    public CanvasRenderer curtain;
    public const int EnumMax_OurForces = (int)OurForces_Sprite.Researcher+1;//얘는 나중에 아군캐릭터 추가되면 바꿔줘야됨
    public const int EnumMax_Traps = (int)Traps_body.Trap_W+1;//얘도 함정 추가되면 바꿔줘야됨
    //DayTime은 추후에 json 파일에서 읽어올 수 있음.
    // Use this for initialization
    void Awake() {            
        CreatedEnemy = new List<Enemy>();
        DeadEnemy = new List<Enemy>();
        isDay = false;//밤부터 시작
    }
    void Start() {
    }

    void ObjectInfoSyncToObject(GameObject g, int Type) {//ObjectInfo 정보 변경 시 아래 Object에 정보 전달
        //수리, 회복 등에서 사용
        ObjectInfo info;
        switch (Type) {
            case (int)ObjectType.Friendly:
                info = GetComponent<ObjectInfo>();
                Friendly f = GetComponentInChildren<Friendly>(true);
                f.Level = info.level;
                f.Hp = info.presentHP;
                f.MaxHp = info.totalHP;
                break;
        }
    }
    // Update is called once per frame
    void Update() {
        if (EnemyClusterManager.IsStageEnd && isDay) {
            changeState();
            GameManager.GenerateComplete = false;
        }
    }

    public void changeState() {
        isDay = !isDay;

        roomManager.OnOffHitCollider();

        ReadyButton.SetActive(!isDay);
        InActReadyButton.SetActive(isDay);

        if (!isDay)
        {
            resultPopUp.gameObject.SetActive(true);
            resultPopUp.InitResultPopUp();
        }

        if (isDay) {
            SoundManager.soundManager.ChangeBGM("4_DAY START");
            curtain.transform.Rotate(0, 90, 0);
            ClearEnemyData();
        }
        else {
            curtain.transform.Rotate(0, -90, 0);
        }
        
        TrapEnable(isDay);
        if (FindObjectOfType<TaskManager>() == null)
            EnemyEnable(isDay);
        CharacterEnable(isDay);
    }
    
    public void ClearEnemyData() {
        CreatedEnemy.Clear();
        DeadEnemy.Clear();
    }

    public void CharacterEnable(bool Day) {
        GameObject[] OurForces = GameObject.FindGameObjectsWithTag("Friendly");
        GameObject spriteObject=null;
        GameObject animationObject = null;
        for (int i = 0; i < OurForces.Length; i++) {
            try
            {
                getOurForces(OurForces[i], ref spriteObject, ref animationObject);
                if (Day)
                {
                    if (spriteObject != null)
                    {
                        setOriginalPoint(ref animationObject, ref spriteObject);
                        spriteObject.SetActive(false);
                        animationObject.SetActive(true);
                        //ObjectInfoSyncToObject(OurForces[i], (int)ObjectType.Friendly);
                    }
                }
                else
                {
                    if (spriteObject != null)
                    {
                        spriteObject.SetActive(true);
                        Friendly F = animationObject.GetComponentInChildren<Friendly>();
                        for (int j = 0; j < ResourceManager_Player.ClusterMax; j++) {
                            F.ClusterDamageStack[j] = 0;
                        }
                        animationObject.SetActive(false);
                        //ObjectInfoSyncToObject(OurForces[i], (int)ObjectType.Friendly);
                    }
                }
            }
            catch { }
        }
    }

    public void EnemyEnable(bool Day) {
        if (Day) {
            GameManager.current.EnemyGenerate();
        }
    }

    public void TrapEnable(bool Day)
    {
        GameObject[] Traps = GameObject.FindGameObjectsWithTag("Trap");
        GameObject trapObject = null;
        for (int i = 0; i < Traps.Length; i++)
        {
            try
            {
                getTraps(Traps[i], ref trapObject);
                if (Day)
                {
                    if (trapObject!= null)
                    {
                        trapObject.GetComponent<SphereCollider>().enabled = true;
                    }
                }
                else
                {
                    if (trapObject != null)
                    {
                        trapObject.GetComponent<SphereCollider>().enabled = false;
                    }
                }
            }
            catch { }
        }
    }

    public void getOurForces(GameObject OurForces, ref GameObject spriteObject, ref GameObject animationObject) {
        spriteObject = null;
        animationObject = null;
        for (int i = 0; i <EnumMax_OurForces;i++) {
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

    public void getTraps(GameObject Traps, ref GameObject trapObject)
    {
        trapObject = null;
        for (int i = 0; i < EnumMax_Traps; i++)
        {
            try
            {
                trapObject = Traps.transform.FindChild(((Traps_body)i).ToString()).gameObject;
            }
            catch { }
            if (trapObject != null)
            {
                break;
            }
        }
    }
}