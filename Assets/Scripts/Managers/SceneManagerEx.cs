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

    public BaseScene[] GetCurrentScenes { get => GameObject.FindObjectsByType<BaseScene>(FindObjectsSortMode.None); }
    public Define.Scene CurrentScene => GetCurrentScene.CurrentScene;
    public Define.Scene NextScene => _nextScene;

    public bool[] LoadingSceneTaskChecker => _loadingSceneTaskChecker;
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

    public void NetworkLoadScene(Define.Scene nextscene,Action<ulong> clientLoadedEvent, Action allPlayerLoadedEvent)
    {
        Managers.RelayManager.NGO_RPC_Caller.AllClientDisconnetedVivoxAndLobbyRpc();
        Managers.RelayManager.NGO_RPC_Caller.OnBeforeSceneUnloadRpc();
        Managers.RelayManager.NetworkManagerEx.SceneManager.OnLoadComplete += SceneManager_OnLoadCompleteAsync;
        Managers.RelayManager.NetworkManagerEx.SceneManager.LoadScene(GetEnumName(nextscene), UnityEngine.SceneManagement.LoadSceneMode.Single);

        void SceneManager_OnLoadCompleteAsync(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
        {
            if (sceneName == nextscene.ToString() && loadSceneMode == LoadSceneMode.Single)
            {
                clientLoadedEvent?.Invoke(clientId);
                Managers.RelayManager.NGO_RPC_Caller.LoadedPlayerCount++;
            }

            if (Managers.RelayManager.NGO_RPC_Caller.LoadedPlayerCount == Managers.RelayManager.CurrentUserCount)
            {
                Managers.RelayManager.NGO_RPC_Caller.IsAllPlayerLoaded = true;//로딩창 90% 이후로 넘어가게끔
                allPlayerLoadedEvent?.Invoke();

                Managers.RelayManager.NetworkManagerEx.SceneManager.OnLoadComplete -= SceneManager_OnLoadCompleteAsync;
            }
        }
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
