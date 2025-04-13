using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerEx:IManagerIResettable,IManagerInitializable
{ 
    private Define.Scene _currentSceneName;
    private LoadingScene _loadingScene;
    private Define.Scene _nextSceneName;
    private bool[] _loadingSceneTaskChecker;

    public BaseScene GetCurrentScene { get => GameObject.FindAnyObjectByType<BaseScene>(); }
    public Define.Scene CurrentSceneName => _currentSceneName;
    public Define.Scene NextSceneName => _nextSceneName;
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
        _currentSceneName = scene;
        SceneManager.LoadScene(GetEnumName(scene));
    }
    public void SetCheckTaskChecker(bool[] CheckTaskChecker)
    {
        _loadingSceneTaskChecker = CheckTaskChecker;
    }


    public void LoadSceneWithLoadingScreen(Define.Scene Nextscene)
    {
        _nextSceneName = Nextscene;
        LoadScene(Define.Scene.LoadingScene);
    }

    public async Task NetworkLoadSceneAsync(Define.Scene scene)
    {
        //Managers.Clear();
        _currentSceneName = scene;
        await Managers.LobbyManager.LeaveCurrentLobby();
        Managers.RelayManager.NetworkManagerEx.SceneManager.LoadScene(GetEnumName(scene), UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    public async Task NetworkLoadSceneAsyncWithLoadingScene(Define.Scene scene)
    {
        Managers.Clear();
        _currentSceneName = scene;
        await Managers.LobbyManager.LeaveCurrentLobby();
        await Managers.VivoxManager.LogoutOfVivoxAsync();//TODO: 나중에 비복스 쓸 때 있으면 이 부분 없앨것
        //Managers.
        //LoadingScene.LoadNextScene(_currentSceneName);
        //여기에 RPC콜러로 모든 클라이언트에 다음씬 설정
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
