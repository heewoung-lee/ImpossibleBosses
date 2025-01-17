using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerEx:IManagerIResettable,IManagerInitializable
{ 
 
    public BaseScene CurrentScene { get => GameObject.FindAnyObjectByType<BaseScene>(); }

    private LoadingScene _loadingScene;
    public void LoadScene(Define.Scene scene)
    {
        Managers.Clear();
        SceneManager.LoadScene(GetEnumName(scene));
    }

    public void LoadSceneWithLoadingScreen(Define.Scene Nextscene)
    {
        LoadingScene.LoadNextScene(Nextscene);
        LoadScene(Define.Scene.LoadingScene);
    }

    public string GetEnumName(Define.Scene type)
    {
        string name = System.Enum.GetName(typeof(Define.Scene), type);
        return name;
    }

    public void Clear()
    {
        CurrentScene.Clear();   
    }

    public void Init()
    {
    }
}
