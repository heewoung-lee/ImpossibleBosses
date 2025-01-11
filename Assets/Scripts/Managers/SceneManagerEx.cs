using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerEx:IManagerIResettable,IManagerInitializable
{ 
 
    public BaseScene currentScene { get => GameObject.FindAnyObjectByType<BaseScene>(); }

    private LoadingScene _loadingscene;


    public void LoadScene(Define.Scene scene)
    {
        Managers.Clear();
        SceneManager.LoadScene(GetEnumName(scene));
    }

    public void LoadSceneWithLoadingScreen(Define.Scene Nextscene)
    {
        Managers.Clear();
        LoadingScene.LoadNextScene(Nextscene);
        SceneManager.LoadScene(GetEnumName(Define.Scene.LoadingScene));
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

    public void Init()
    {
    }
}
