using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerEx:IManagerIResettable,IManagerInitializable
{ 
    private Define.Scene _currentScene;
    private LoadingScene _loadingScene;
    private Define.Scene _nextScene;
    private bool[] _loadingSceneTaskChecker;

    public BaseScene GetCurrentScene { get => GameObject.FindAnyObjectByType<BaseScene>(); }
    public Define.Scene CurrentScene => _currentScene;
    public Define.Scene NextScene => _nextScene;

    public void SetNextScene(Define.Scene nextScene)
    {
        _nextScene = nextScene;
    }


    public bool[] LoadingSceneTaskChecker
    {
        get
        {
            bool[] loadingChecker = _loadingSceneTaskChecker;
            _loadingSceneTaskChecker = null;
            return loadingChecker;
        }
    }
    public void LoadScene(Define.Scene scene)
    {
        Managers.Clear();
        _currentScene = scene;
        SceneManager.LoadScene(GetEnumName(scene));
    }
    public void SetCheckTaskChecker(bool[] CheckTaskChecker)
    {
        _loadingSceneTaskChecker = CheckTaskChecker;
    }


    public void LoadSceneWithLoadingScreen(Define.Scene Nextscene)
    {
        _nextScene = Nextscene;
        LoadScene(Define.Scene.LoadingScene);
    }

    public async Task NetworkLoadSceneAsync(Define.Scene scene)
    {
        //Managers.Clear();
        _currentScene = scene;
        await Managers.LobbyManager.LeaveCurrentLobby();
        Managers.RelayManager.NetworkManagerEx.SceneManager.LoadScene(GetEnumName(scene), UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    public void NetworkLoadSceneAsyncWithLoadingScene(Define.Scene scene)
    {
        Managers.Clear();
        _currentScene = scene;
        _nextScene = Define.Scene.GamePlayScene;
        Managers.RelayManager.NetworkManagerEx.SceneManager.LoadScene(Define.Scene.NetworkLoadingScene.ToString(), UnityEngine.SceneManagement.LoadSceneMode.Single);
        //먼저 모든 클라이언트가 로딩씬에 로드 되게 한 다음
        //로딩이 끝나면 
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
