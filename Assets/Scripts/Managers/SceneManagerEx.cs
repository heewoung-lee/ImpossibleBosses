using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerEx:IManagerIResettable
{ 
 
    //현재 베이스 씬을 가져온다.
    //LoadScene 씬을 클리어한후 로드한다.
    //Enum을 이름으로 바꿔주는 메서드
    public BaseScene currentScene { get => GameObject.FindAnyObjectByType<BaseScene>(); }

    public void LoadScene(Define.Scene scene)
    {
        Managers.Clear();
        SceneManager.LoadScene(GetEnumName(scene));
    }

    public string GetEnumName(Define.Scene type)
    {
        string name = System.Enum.GetName(typeof(Define.Scene), type);
        return name;
    }

    public void Clear()
    {
        currentScene.Clear();   
    }
}
