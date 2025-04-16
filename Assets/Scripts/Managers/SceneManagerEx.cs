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
    public void LoadScene(Define.Scene nextscene)
    {
        Managers.Clear();
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

    public void NetworkLoadSceneAsync(Define.Scene nextscene)
    {
        Managers.Clear();
        Managers.RelayManager.NGO_RPC_Caller.AllClientDisconnetedVivoxAndLobbyRpc();
        Managers.RelayManager.NetworkManagerEx.SceneManager.LoadScene(GetEnumName(nextscene), UnityEngine.SceneManagement.LoadSceneMode.Single);
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
