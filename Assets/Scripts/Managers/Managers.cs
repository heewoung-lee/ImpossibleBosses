using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Managers : MonoBehaviour
{
    private static Managers _instance;

    private static Managers Instance
    {
        get
        {
            Init();
            return _instance;
        }
    }
    private BufferManager _bufferManager = new BufferManager();
    public static BufferManager BufferManager { get => Instance._bufferManager; }

    private DataManager _dataManager = new DataManager();
    public static DataManager DataManager { get => Instance._dataManager; }

    private GameManagerEx _gameManagerEx = new GameManagerEx();
    public static GameManagerEx GameManagerEx { get => Instance._gameManagerEx; }

    private InputManager _inputManager = new InputManager();
    public static InputManager InputManager { get => Instance._inputManager; }

    private ItemDataManager _itemDataManaer = new ItemDataManager();
    public static ItemDataManager ItemDataManager { get => Instance._itemDataManaer; }

    private LootItemManager _lootItemManaer = new LootItemManager();
    public static LootItemManager LootItemManager { get=>  Instance._lootItemManaer; }
    
    private PoolManager _poolManager = new PoolManager();
    public static PoolManager PoolManager { get => Instance._poolManager; }

    private ResourceManager _resourceManager = new ResourceManager();
    public static ResourceManager ResourceManager { get => Instance._resourceManager; }
    
    private SceneManagerEx _sceneManagerEx = new SceneManagerEx();
    public static SceneManagerEx SceneManagerEx { get => Instance._sceneManagerEx; }

    private SoundManager _soundManager = new SoundManager();
    public static SoundManager SoundManager { get => Instance._soundManager; }

    private UI_Manager _ui_manager = new UI_Manager();
    public static UI_Manager UI_Manager { get => Instance._ui_manager; }

    private VFXManager _vFX_Manager = new VFXManager();
    public static VFXManager VFX_Manager { get => Instance._vFX_Manager; }

    public static Coroutine ManagersStartCoroutine(IEnumerator couroutine)
    {
        return _instance.StartCoroutine(couroutine);
    } 

    public static void ManagersStopCoroutine(IEnumerator couroutine)
    {
        _instance.StopCoroutine(couroutine);
    }

    private static void Init()
    {
        if (_instance == null)
        {
            GameObject go = GameObject.Find("@Managers");
            if (go == null)
            {
                go = new GameObject() { name = "@Managers" };
                go.AddComponent<Managers>();
            }
            DontDestroyOnLoad(go);
            _instance = go.GetComponent<Managers>();
            _instance._inputManager.Init();
            _instance._dataManager.Init();
            _instance._poolManager.Init();
            _instance._soundManager.Init();
            _instance._gameManagerEx.Init();
            _instance._itemDataManaer.Init();
            _instance._bufferManager.Init();
        }

    }


    public static void Clear()
    {
        _instance._soundManager.Clear();
        _instance._ui_manager.Clear();
        _instance._sceneManagerEx.Clear();
        _instance._poolManager.Clear();
    }
}
