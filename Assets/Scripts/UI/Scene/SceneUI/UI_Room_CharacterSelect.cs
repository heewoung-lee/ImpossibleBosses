using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Lobbies;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UI;

public class UI_Room_CharacterSelect : UI_Scene
{
    private const int MAX_PLAYER_COUNT = 8;
    enum Transforms
    {
        CharactorSelectTr
    }
    enum GameObjects
    {
        LoadingPanel
    }
    enum Buttons
    {
        BackToLobbyButton,

    }
    private GameObject _playerSelector;
    private Transform _charactorSelectTr;
    private UI_RoomPlayerFrame[] _ui_RoomPlayerFrames;
    private Button _backToLobbyButton;
    private GameObject _loadingPanel;
    private UI_LoadingPanel _ui_LoadingPanel;
    private GameObject _ui_CharactorSelectRoot;

    public GameObject PlayerSelector { get { return _playerSelector; } }

    public GameObject UI_CharactorSelectRoot
    {
        get
        {
            if(_ui_CharactorSelectRoot == null)
            {
                _ui_CharactorSelectRoot = new GameObject() {name = "NGO_ROOT" };
                if(Managers.RelayManager.NetWorkManager.IsListening == true)
                {
                    NetworkObject networkObject = _ui_CharactorSelectRoot.AddComponent<NetworkObject>();
                    networkObject.Spawn();
                }
                _ui_CharactorSelectRoot.AddComponent<RectTransform>();
                Canvas ui_canvas = _ui_CharactorSelectRoot.AddComponent<Canvas>();
                ui_canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                ui_canvas.sortingOrder = 100;
                
            }
            return _ui_CharactorSelectRoot;
        }
    }


    protected override void AwakeInit()
    {
        base.AwakeInit();
        Bind<Transform>(typeof(Transforms));
        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));
        _playerSelector = Managers.ResourceManager.InstantiatePrefab("Map/ChoosePlayer");
        _charactorSelectTr = Get<Transform>((int)Transforms.CharactorSelectTr);
        _backToLobbyButton = Get<Button>((int)Buttons.BackToLobbyButton);
        _backToLobbyButton.onClick.AddListener(async () =>
        {
            await BacktoLobby();
        });
        _loadingPanel = Get<GameObject>((int)GameObjects.LoadingPanel);
        
        _ui_RoomPlayerFrames = new UI_RoomPlayerFrame[MAX_PLAYER_COUNT];
        for (int index = 0; index < _ui_RoomPlayerFrames.Length; index++)
        {
            _ui_RoomPlayerFrames[index] = Managers.UI_Manager.MakeSubItem<UI_RoomPlayerFrame>(_charactorSelectTr);
        }
    }

    protected override void OnEnableInit()
    {
        base.OnEnableInit();
        _loadingPanel.SetActive(false);
    }

    public async Task BacktoLobby()
    {
        try
        {
            _loadingPanel.SetActive(true);
            await Managers.LobbyManager.TryJoinLobbyByNameOrCreateWaitLobby();
            Managers.SceneManagerEx.LoadScene(Define.Scene.LobbyScene);
            Debug.Log($"{Managers.LobbyManager.CurrentLobby.Name}");
           
        }
        catch (Exception error)
        {
            Debug.Log($"에러코드{error}");
        }

    }
    protected override void StartInit()
    {
        base.StartInit();
        _ui_LoadingPanel = Managers.UI_Manager.GetSceneUIFromResource<UI_LoadingPanel>();
        SpawnPlayerSelector();
    }

    private void SpawnPlayerSelector()
    {
        GameObject targetFrame = _ui_RoomPlayerFrames[Managers.RelayManager.NetWorkManager.LocalClientId].gameObject;
        RectTransform targetFrame_Rect = targetFrame.GetComponent<RectTransform>();

        GameObject chractorSeletor = Managers.RelayManager.SpawnCharactor_Selector(Managers.RelayManager.NetWorkManager.LocalClientId);
        RectTransform chractorSeletor_Rect = chractorSeletor.GetComponent<RectTransform>();

        Vector2 frame_size = GetUISize(targetFrame);
        Vector2 frame_screenPos = GetUIScreenPosition(targetFrame_Rect);


        // 부모 설정
        chractorSeletor_Rect.SetParent(UI_CharactorSelectRoot.transform,false);
        chractorSeletor_Rect.sizeDelta = frame_size;
        chractorSeletor_Rect.position = frame_screenPos;

    }



    //TODO:테스트하면 이거 지워야함
    private void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 100, 100), "GetJoinCode"))
        {
            Debug.Log($"내 조인코드는 {Managers.RelayManager.JoinCode}");
            Debug.Log($"로비의 조인코드는{Managers.LobbyManager.CurrentLobby.Data["RelayCode"].Value}");
        }
    }

}
