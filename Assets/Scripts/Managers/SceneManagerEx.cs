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

    private Action _onBeforeSceneUnloadLocalEvent;


    public event Action OnBeforeSceneUnloadLocalEvent
    {
        add
        {
            UniqueEventRegister.AddSingleEvent(ref _onBeforeSceneUnloadLocalEvent, value);
        }
        remove
        {
            UniqueEventRegister.RemovedEvent(ref _onBeforeSceneUnloadLocalEvent, value);
        }
    }
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
    public void InvokeOnBeforeSceneUnloadLocalEvent()
    {
        _onBeforeSceneUnloadLocalEvent?.Invoke();
        Debug.Log("씬 로드 되기전 호출");
    }

    public void NetworkLoadScene(Define.Scene nextscene,Action<ulong> clientLoadedEvent, Action allPlayerLoadedEvent)
    {
        Managers.RelayManager.NGO_RPC_Caller.OnBeforeSceneUnloadLocalRpc();//모든 플레이어가 씬 호출전 실행해야할 이벤트(로컬 각자가 맡음)
        Managers.RelayManager.NGO_RPC_Caller.OnBeforeSceneUnloadRpc();//모든 플레이어가 씬 호출전 실행해야할 넷워크 오브젝트 초기화(호스트가 맡음)
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
