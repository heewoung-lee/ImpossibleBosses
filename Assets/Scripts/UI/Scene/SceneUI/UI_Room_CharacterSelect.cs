using System;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Events;
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
        Button_Ready
    }
    private GameObject _playerSelector;
    private Transform _charactorSelectTr;
    private UI_RoomPlayerFrame[] _ui_RoomPlayerFrames;
    private Button _backToLobbyButton;
    private GameObject _loadingPanel;
    private UI_LoadingPanel _ui_LoadingPanel;
    private GameObject _ui_CharactorSelectRoot;
    private NetworkManager _netWorkManager;
    private Button _button_Ready;

    public GameObject PlayerSelector { get { return _playerSelector; } }
    


    public GameObject UI_CharactorSelectRoot
    {
        get
        {
            if(_ui_CharactorSelectRoot == null)
            {
                _ui_CharactorSelectRoot = Managers.ResourceManager.InstantiatePrefab("NGO/NGO_ROOT");
                Managers.RelayManager.SpawnNetworkOBJ(_netWorkManager.LocalClientId, _ui_CharactorSelectRoot);
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
        _netWorkManager = Managers.RelayManager.NetWorkManager;
        _button_Ready = Get<Button>((int)Buttons.Button_Ready);
    }



    protected override void OnEnableInit()
    {
        base.OnEnableInit();
        _loadingPanel.SetActive(false);
        _netWorkManager.OnClientConnectedCallback += EntetedPlayerinLobby;
        _netWorkManager.OnClientDisconnectCallback += DisConnetedPlayerinLobby;
        Managers.RelayManager.DisconnectPlayerEvent = Managers.LobbyManager.DisconnetPlayerinRoom;
    }

    public void SetButtonEvent(UnityAction action)
    {
        _button_Ready.onClick.AddListener(action);
    }

    private void DisConnetedPlayerinLobby(ulong playerIndex)
    {
        Debug.Log("플레이어가 나갔습니다.");
    }

    public void EntetedPlayerinLobby(ulong playerIndex)
    {
        Debug.Log("EnteredPlayerinLobby 이벤트 발생");
        SpawnChractorSeletorAndSetPosition(playerIndex);
    }
    

    public async Task BacktoLobby()
    {
        try
        {
            _loadingPanel.SetActive(true);
            _netWorkManager.OnClientConnectedCallback -= EntetedPlayerinLobby;
            _netWorkManager.OnClientDisconnectCallback -= DisConnetedPlayerinLobby;
            Managers.RelayManager.DisconnectPlayerEvent = null;
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
        SpawnChractorSeletorAndSetPosition(_netWorkManager.LocalClientId);
    }


    private void SpawnChractorSeletorAndSetPosition(ulong playerIndex)
    {
        if (_netWorkManager.IsHost)
        {
            GameObject characterSelector = Managers.ResourceManager.InstantiatePrefab("NGO/Character_Select_Rect");
            characterSelector = SetPositionCharacterSelector(characterSelector,playerIndex);
        }
    }

    private GameObject SetPositionCharacterSelector(GameObject characterSelector,ulong playerIndex)
    {
        int playerframeindex = 0; 
        for (int i = 0; i< _ui_RoomPlayerFrames.Length; i++)
        {
            if (_ui_RoomPlayerFrames[i].ChracterSelectorNGO == null)
            {
                _ui_RoomPlayerFrames[i].SetCharacterSelector(characterSelector);
                playerframeindex = i;
                break;
            }
        }

        GameObject targetFrame = _ui_RoomPlayerFrames[playerframeindex].gameObject;
        RectTransform targetFrame_Rect = targetFrame.GetComponent<RectTransform>();

        RectTransform chractorSeletor_Rect = characterSelector.GetComponent<RectTransform>();

        Vector2 frame_size = GetUISize(targetFrame);
        Vector2 frame_screenPos = GetUIScreenPosition(targetFrame_Rect);

        chractorSeletor_Rect.sizeDelta = frame_size;
        chractorSeletor_Rect.position = frame_screenPos;

        GameObject characterSelecter = Managers.RelayManager.SpawnNetworkOBJ(playerIndex, characterSelector, UI_CharactorSelectRoot.transform);

        return characterSelecter;
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
