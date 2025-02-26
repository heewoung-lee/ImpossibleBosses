using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerEx:IManagerIResettable,IManagerInitializable
{ 
    private Define.Scene _currentSceneName;
    private LoadingScene _loadingScene;
    public BaseScene GetCurrentScene { get => GameObject.FindAnyObjectByType<BaseScene>(); }
    public Define.Scene CurrentSceneName => _currentSceneName;
    public void LoadScene(Define.Scene scene)
    {
        Managers.Clear();
        _currentSceneName = scene;
        SceneManager.LoadScene(GetEnumName(scene));
    }

    public void LoadSceneWithLoadingScreen(Define.Scene Nextscene)
    {
        LoadingScene.LoadNextScene(Nextscene);
        LoadScene(Define.Scene.LoadingScene);
    }

    public void NetworkLoadScene(NetworkManager networkManager,Define.Scene scene)
    {
        Managers.Clear();
        networkManager.SceneManager.LoadScene(GetEnumName(scene), UnityEngine.SceneManagement.LoadSceneMode.Single);
    }


    public string GetEnumName(Define.Scene type)
    {
        string name = System.Enum.GetName(typeof(Define.Scene), type);
        return name;
    }

    public void Clear()
    {
        GetCurrentScene.Clear();
        Managers.UI_Manager.Clear();
    }

    public void Init()
    {
    }
}
