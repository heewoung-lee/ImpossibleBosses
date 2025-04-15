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

    private Action<NetworkLoadingScene> _loadedSceneActionEvent;
    private Action<BaseScene> _nextSceneActionEvent;


    private int _playerLoadedLoadingSceneCount = 0;

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
    public void LoadScene(Define.Scene nextscene)
    {
        Managers.Clear();
        _nextScene = nextscene;
        SceneManager.LoadScene(GetEnumName(nextscene));
    }
    public void SetCheckTaskChecker(bool[] CheckTaskChecker)
    {
        _loadingSceneTaskChecker = CheckTaskChecker;
    }


    public void LoadSceneWithLoadingScreen(Define.Scene nextscene)
    {
        _nextScene = nextscene;
        LoadScene(Define.Scene.LoadingScene);
    }

    public async Task NetworkLoadSceneAsync(Define.Scene nextscene)
    {
        Managers.Clear();
        _nextScene = nextscene;
        await Managers.LobbyManager.LeaveCurrentLobby();
        Managers.RelayManager.NetworkManagerEx.SceneManager.LoadScene(GetEnumName(nextscene), UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    public void NetworkLoadSceneAsyncWithLoadingScene(Define.Scene nextscene,Action<NetworkLoadingScene> loadingSceneLoadedEvent,Action<BaseScene> nextSceneLoadedEvent)
    {
        Managers.Clear();
        _nextScene = nextscene;
        NetworkManager.Singleton.SceneManager.OnLoadComplete += OnClientFinishedLoading;
        Managers.RelayManager.NetworkManagerEx.SceneManager.LoadScene(Define.Scene.NetworkLoadingScene.ToString(), UnityEngine.SceneManagement.LoadSceneMode.Single);
        _loadedSceneActionEvent = loadingSceneLoadedEvent;
        _nextSceneActionEvent = nextSceneLoadedEvent;
        //먼저 모든 클라이언트가 로딩씬에 로드 되게 한 다음
        //로딩이 끝나면 
        //로딩인원 카운트가 되면 할 이벤트 설정

    }

    private void OnClientFinishedLoading(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
    {
        Debug.Log($"여기는 거칠까?{sceneName} 유저ID: {clientId}");

        if(sceneName == Define.Scene.NetworkLoadingScene.ToString() && loadSceneMode == LoadSceneMode.Single)
        {
            Scene loadingScene = SceneManager.GetSceneByName(sceneName);
            NetworkLoadingScene networkLoadingScene = GetBaseSceneFromScene(loadingScene) as NetworkLoadingScene;
            _loadedSceneActionEvent.Invoke(networkLoadingScene);
            _playerLoadedLoadingSceneCount++;
        }


        if (_playerLoadedLoadingSceneCount == Managers.RelayManager.CurrentUserCount)//
        {
            Debug.Log("AdditiveScnee");
            Managers.RelayManager.NetworkManagerEx.SceneManager.LoadScene(Define.Scene.GamePlayScene.ToString(), UnityEngine.SceneManagement.LoadSceneMode.Additive);
            Scene nextScene = SceneManager.GetSceneByName(sceneName);
            BaseScene baseScene = GetBaseSceneFromScene(nextScene);
            _nextSceneActionEvent.Invoke(baseScene);//카메라 없앰
            _playerLoadedLoadingSceneCount = 0;
        }

        if (loadSceneMode == LoadSceneMode.Additive && sceneName == _nextScene.ToString())
        {
            Debug.Log($"{clientId}유저 씬로드 완료");
            Scene loadingScene = SceneManager.GetSceneByName(Define.Scene.NetworkLoadingScene.ToString());
            //Managers.RelayManager.NetworkManagerEx.SceneManager.UnloadScene(loadingScene);
        }

    }
    public BaseScene GetBaseSceneFromScene(Scene scene)
    {
        if (!scene.IsValid() || !scene.isLoaded)
            return null;

        foreach (GameObject rootObj in scene.GetRootGameObjects())
        {
            BaseScene baseScene = rootObj.GetComponentInChildren<BaseScene>(true);
            if (baseScene != null)
                return baseScene;
        }
        return null;
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
    public Camera FindMainCameraInScene(Scene scene)
    {
        if (!scene.IsValid() || !scene.isLoaded)
            return null;

        foreach (GameObject rootObj in scene.GetRootGameObjects())
        {
            Camera cam = rootObj.GetComponentInChildren<Camera>(true);
            if (cam != null && cam.CompareTag("MainCamera"))
                return cam;
        }

        return null;
    }
    public void Init()
    {
    }
}
