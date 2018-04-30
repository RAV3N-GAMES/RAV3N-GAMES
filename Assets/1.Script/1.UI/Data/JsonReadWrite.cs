using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using LitJson;

/*
 * JsonReadWrite를 MonoBehavior 대신 상속해서 쓰시면 됩니다.
 */
public class JsonReadWrite : MonoBehaviour
{
    /* ReadMain 함수
     * 가급적이면 Awake 함수 안에서 호출하시기 바랍니다.
     * 1. ParsingJson 함수는 override.
     * 2. ReadMain 함수의 인자 Path 설정
     * 3. ReadMain 함수 호출
     * 순으로 사용하여 Json file을 읽을 수 있습니다.
     */
    public void ReadMain(String Path)
    {
        string OriginalFile = ReadFile(Path);
        JsonParse(OriginalFile);
    }

    /* ReadFile 함수
     * 파일을 읽어옴. Path는 Unity Project의 Sector를 기본으로 상대경로로 지정 
     * ex) Sector\Assets\1.Script\Player\PlayerDetail.json을 읽고 싶을 경우
     * -> private const string Path="./Assets/1.Script/Player/PlayerDetail.json"
     */
    public string ReadFile(string Path)
    {
        TextAsset textAsset = Resources.Load(Path) as TextAsset;
        return textAsset.ToString();
    }
    public void JsonParse(string File)
    {
        StartCoroutine(LoadCo(File));
    }

    public IEnumerator LoadCo(string File)
    {
        JsonData data = JsonMapper.ToObject(File);
        ParsingJson(data);
        yield return null;
    }

    /*
     * ParsingJson 함수는 override해서 사용하세요
     * ex) public override void ParsingJson(JsonData data){
     *   //함수 내용(data를 적절한 객체에 할당)
     * }
     */
    /*
     * 함수 내부 형태는 보통 다음을 따를 것입니다.
     *         for (int i = 0; i < data.Count; i++) {
           Data_SetupPlayer tmp = new Data_SetupPlayer();
           tmp.fame = int.Parse(data[i]["fame"].ToString());
           ...
           Tbl_Player.Add(tmp);
       }
       * Data_SetupPlayer는 해당 Json 파일의 데이터를 저장하기 위해 정의한 public class입니다.
       * Tbl_Player는 Data_SetupPlayer List입니다.
     */

    public virtual void ParsingJson(JsonData data)
    {
        Debug.Log("Please Override this function.");
        Debug.Log("like public override void ParsingJson(JsonData data){ //statements... }");
    }


    public void WriteMain(String Path, List<JsonData> data)
    {
        WriteFile(Path, data);
    }

    public void WriteFile(String Path, List<JsonData> data)
    {
        File.WriteAllText(Path, data.ToString());
    }
}
