using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour {
    public static Dictionary<string,GameObject> Wall;
    public static Dictionary<string, GameObject> Monster;
    public static Dictionary<string, GameObject> Trap;
    public static Dictionary<string, GameObject> Treasure;

    public static Dictionary<string, GameObject> WallImage;
    public static Dictionary<string, GameObject> MonsterImage;
    public static Dictionary<string, GameObject> TrapImage;
    public static Dictionary<string, GameObject> TreasureImage;

    // Use this for initialization
    void Awake () {
        Wall = new Dictionary<string, GameObject>();
        Monster = new Dictionary<string, GameObject>();
        Trap = new Dictionary<string, GameObject>();
        Treasure = new Dictionary<string, GameObject>();

        WallImage = new Dictionary<string, GameObject>();
        MonsterImage = new Dictionary<string, GameObject>();
        TrapImage = new Dictionary<string, GameObject>();
        TreasureImage = new Dictionary<string, GameObject>();

        InitObject(Wall, WallImage, "");
        InitObject(Monster, MonsterImage, "");
        InitObject(Trap, TrapImage, "");
        InitObject(Treasure, TreasureImage, "");
	}

    void InitObject(Dictionary<string,GameObject> Obj, Dictionary<string,GameObject> Img, string path)
    {
        //GameObject[] loadObj = Resources.LoadAll(path) as GameObject[];
        //
        //for(int i = 0; i < loadObj.Length; i++)
        //{
        //    Obj.Add(loadObj[i].name, loadObj[i]);
        //}
    }
}
