using System;
using System.Collections;
using UnityEngine;
using Zenject;

namespace GameManagers
{
    public class Managers : MonoBehaviour
    {
        private static Managers _instance;
        private static bool _isQuitting;
        private static Managers Instance
        {
            get
            {
                Init();
                return _instance;
            }
        }
        // private BufferManager _bufferManager = new BufferManager();
        // public static BufferManager BufferManager { get => Instance._bufferManager; }
        //
        // private DataManager _dataManager = new DataManager();
        // public static DataManager DataManager { get => Instance._dataManager; }

        private GameManagerEx _gameManagerEx = new GameManagerEx();
        public static GameManagerEx GameManagerEx { get => Instance._gameManagerEx; }

        // private InputManager _inputManager = new InputManager();
        // public static InputManager InputManager { get => Instance._inputManager; }

        // private ItemDataManager _itemDataManaer = new ItemDataManager();
        // public static ItemDataManager ItemDataManager { get => Instance._itemDataManaer; }

        private LobbyManager _lobbyManager = new LobbyManager();
        public static LobbyManager LobbyManager { get => Instance._lobbyManager; }

        private LogInManager _logInManager = new LogInManager();
        public static LogInManager LogInManager { get => Instance._logInManager; }

        private LootItemManager _lootItemManaer = new LootItemManager();
        public static LootItemManager LootItemManager { get=>  Instance._lootItemManaer; }
   
        private NgoPoolManager _ngoPoolManager = new NgoPoolManager();
        public static NgoPoolManager NgoPoolManager { get => Instance._ngoPoolManager; }

        private RelayManager _relayManager = new RelayManager();
        public static RelayManager RelayManager { get => Instance._relayManager; }
    
        private PoolManager _poolManager = new PoolManager();
        public static PoolManager PoolManager { get => Instance._poolManager; }

        // private ResourceManager _resourceManager;
        // public static ResourceManager ResourceManager => Instance._resourceManager;

        private SceneDataSaveAndLoader _sceneDataSaveAndLoader = new SceneDataSaveAndLoader();
        public static SceneDataSaveAndLoader SceneDataSaveAndLoader { get => Instance._sceneDataSaveAndLoader; }

        private SceneManagerEx _sceneManagerEx = new SceneManagerEx();
        public static SceneManagerEx SceneManagerEx { get => Instance._sceneManagerEx; }

        // private SkillManager _skillManager = new SkillManager();
        // public static SkillManager SkillManager { get => Instance._skillManager; }

        private SocketEventManager _socketEventManager = new SocketEventManager();
        public static SocketEventManager SocketEventManager { get => Instance._socketEventManager; }

        private SoundManager _soundManager = new SoundManager();
        public static SoundManager SoundManager { get => Instance._soundManager; }

        // private UIManager _uiManager = new UIManager();
        // public static UIManager UIManager { get => Instance._uiManager; }

        private VFXManager _vfxManager = new VFXManager();
        public static VFXManager VFXManager { get => Instance._vfxManager; }

        private VivoxManager _vivoxManager = new VivoxManager();
        public static VivoxManager VivoxManager { get => Instance._vivoxManager; }

        public static Coroutine ManagersStartCoroutine(IEnumerator couroutine)
        {
            return _instance.StartCoroutine(couroutine);
        } 

        public static void ManagersStopCoroutine(IEnumerator couroutineIEnumerator)
        {
            _instance.StopCoroutine(couroutineIEnumerator);
        }
        public static void ManagersStopCoroutine(Coroutine couroutine)
        {
            _instance.StopCoroutine(couroutine);
        }

        public static void ManagersAllstopCoroutine()
        {
            _instance.StopAllCoroutines();
        }

        private static void Init()
        {
            if (_instance == null)
            {
                if (_isQuitting == true)//에디터가 종료될때 다른 클래스들이 Instance를 참조하려해서 오류가 남 종료중일땐 참조를 못하게 막는 방지코드
                    return;

                GameObject go = GameObject.Find("@Managers");
                if (go == null)
                {
                    go = new GameObject() { name = "@Managers" };
                    go.AddComponent<Managers>();
                }
                if (Application.isPlaying)
                {
                    DontDestroyOnLoad(go);
                }
                _instance = go.GetComponent<Managers>();
                //_instance._inputManager.Init();
                //_instance._dataManager.Init();
                _instance._poolManager.Init();
                // _instance._soundManager.Init();
                _instance._gameManagerEx.Init();
                //_instance._itemDataManaer.Init();
                //_instance._bufferManager.Init();
                //_instance._skillManager.Init();
            }

        }

        private void OnDestroy()
        {
            _isQuitting = true;
        }

        public async void OnApplicationQuit()
        {
            if (_socketEventManager == null) return;
            try
            {
                await _socketEventManager.InvokeDisconnectRelayEvent();
                await _socketEventManager.InvokeLogoutVivoxEvent();
                await _socketEventManager.InvokeLogoutAllLeaveLobbyEvent();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            //순서대로 끊어야 함
        }


        public static void Clear()
        {
            // _instance._soundManager.Clear();
            //_instance._uiManager.Clear();
            _instance._sceneManagerEx.Clear();
            _instance._poolManager.Clear();
            //_instance._resourceManager.Clear();
            _instance._ngoPoolManager.Clear();
            //_instance._skillManager.Clear();
        }
    }
}
