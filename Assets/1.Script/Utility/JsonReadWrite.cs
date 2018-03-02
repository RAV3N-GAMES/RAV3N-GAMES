using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using LitJson;

public class JsonReadWrite : MonoBehaviour {
    /*
     * JsonReadWrite 인스턴스 만든 후에 이거 실행시키세요
     * 인스턴스는 new키워드 사용해서 만들어주세요
     * ParsingJson 함수는 override 하셔야합니다.
     * Path도 정해놓고 함수 콜해야합니다.
     */
    public void MainStarter(String Path) {
        string OriginalFile = ReadFile(Path);
        JsonParse(OriginalFile);
    }
    
    //파일을 읽어옴. Path는 Unity Project의 Sector를 기본으로 상대경로로 지정 
    public string ReadFile(string Path) {
        return File.ReadAllText(Path);
    }
    public void JsonParse(string File){
        StartCoroutine(LoadCo(File));
    }

    IEnumerator LoadCo(string File)
    {
        JsonData data = JsonMapper.ToObject(File);
        ParsingJson(data);
        yield return null;
    }

    //override해서 사용하세요
    public virtual void ParsingJson(JsonData data)
    {
        Debug.Log("Please Override this function.");
    }
}
