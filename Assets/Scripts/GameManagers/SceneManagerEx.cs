using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameManagers
{
    public class SceneManagerEx:IManagerIResettable,IManagerInitializable
    { 
        private Define.Scene _currentScene;
        private Define.Scene _nextScene;
        private bool[] _loadingSceneTaskChecker;

        private Action _onBeforeSceneUnloadLocalEvent;
        private Action<ulong> _onClientLoadedEvent;
        private Action _onAllPlayerLoadedEvent;

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
        public event Action<ulong> OnClientLoadedEvent
        {
            add
            {
                UniqueEventRegister.AddSingleEvent(ref _onClientLoadedEvent, value);
            }
            remove
            {
                UniqueEventRegister.RemovedEvent(ref _onClientLoadedEvent, value);
            }
        }
        public event Action OnAllPlayerLoadedEvent
        {
            add
            {
                UniqueEventRegister.AddSingleEvent(ref _onAllPlayerLoadedEvent, value);
            }
            remove
            {
                UniqueEventRegister.RemovedEvent(ref _onAllPlayerLoadedEvent, value);   
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

        public void NetworkLoadScene(Define.Scene nextscene)
        {
            Managers.RelayManager.NgoRPCCaller.OnBeforeSceneUnloadLocalRpc();//모든 플레이어가 씬 호출전 실행해야할 이벤트(로컬 각자가 맡음)
            Managers.RelayManager.NgoRPCCaller.OnBeforeSceneUnloadRpc();//모든 플레이어가 씬 호출전 실행해야할 넷워크 오브젝트 초기화(호스트가 맡음)
            Managers.RelayManager.NetworkManagerEx.SceneManager.OnLoadComplete += SceneManagerOnLoadCompleteAsync;
            Managers.RelayManager.NetworkManagerEx.SceneManager.LoadScene(GetEnumName(nextscene), UnityEngine.SceneManagement.LoadSceneMode.Single);

            void SceneManagerOnLoadCompleteAsync(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
            {
                if (sceneName == nextscene.ToString() && loadSceneMode == LoadSceneMode.Single)
                {
                    _onClientLoadedEvent?.Invoke(clientId);
                    Managers.RelayManager.NgoRPCCaller.LoadedPlayerCount++;
                }

                if (Managers.RelayManager.NgoRPCCaller.LoadedPlayerCount == Managers.RelayManager.CurrentUserCount)
                {
                    Managers.RelayManager.NgoRPCCaller.IsAllPlayerLoaded = true;//로딩창 90% 이후로 넘어가게끔
                    _onAllPlayerLoadedEvent?.Invoke();
                
                    _onClientLoadedEvent = null; // 호출이 끝난뒤 모든 이벤트 구독 전부 삭제
                    _onAllPlayerLoadedEvent = null;
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
            Managers.UIManager.Clear();
        }

        public void Init()
        {
        }
    }
}
