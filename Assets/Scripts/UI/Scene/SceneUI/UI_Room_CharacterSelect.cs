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

struct ReadyButtonImages
{
   public Sprite readyButtonImage;
   public string readyButtonText;
   public Color readyButtonTextColor;
}

public class UI_Room_CharacterSelect : UI_Scene
{
    private const int MAX_PLAYER_COUNT = 8;
   
    enum readyButtonStateEnum
    {
        cancelState,
        trueState
    }
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
        Button_Ready,
        Button_Start
    }
    private Transform _chooseCameraTr;
    private Transform _charactorSelect;
    private UI_RoomPlayerFrame[] _ui_RoomPlayerFrames;
    private Button _backToLobbyButton;
    private GameObject _loadingPanel;
    private UI_LoadingPanel _ui_LoadingPanel;
    private GameObject _ui_CharactorSelectRoot;
    private NetworkManager _netWorkManager;
    private Button _button_Ready;
    private Button _button_Start;
    private TMP_Text _button_Text;
    private CharacterSelectorNGO _chracterSelectorNGO;
    private bool _readyButtonState;

    private ReadyButtonImages[] _readyButtonStateValue;

    public Transform ChooseCameraTr { get => _chooseCameraTr; }
    protected override void AwakeInit()
    {
        base.AwakeInit();
        Bind<Transform>(typeof(Transforms));
        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));
        _chooseCameraTr = Managers.ResourceManager.InstantiatePrefab("Map/ChoosePlayer").GetComponent<Module_ChooseCharactorTr>().ChooseCameraTr;
        _charactorSelect = Get<Transform>((int)Transforms.CharactorSelectTr);
        _backToLobbyButton = Get<Button>((int)Buttons.BackToLobbyButton);
        _button_Start = Get<Button>((int)Buttons.Button_Start);
        _button_Start.onClick.AddListener(async()=> await LoadScenePlayGames());
        _button_Start.gameObject.SetActive(false);
        _backToLobbyButton.onClick.AddListener(async () =>
        {
            await BacktoLobby();
        });
        _loadingPanel = Get<GameObject>((int)GameObjects.LoadingPanel);
        
        _ui_RoomPlayerFrames = new UI_RoomPlayerFrame[MAX_PLAYER_COUNT];
        for (int index = 0; index < _ui_RoomPlayerFrames.Length; index++)
        {
            _ui_RoomPlayerFrames[index] = Managers.UI_Manager.MakeSubItem<UI_RoomPlayerFrame>(_charactorSelect);
        }
        _netWorkManager = Managers.RelayManager.NetWorkManager;
        _button_Ready = Get<Button>((int)Buttons.Button_Ready);
        _button_Text = _button_Ready.GetComponentInChildren<TMP_Text>();
        ReadyButtonInitalize();
    }

    private void ReadyButtonInitalize()
    {
        _readyButtonStateValue = new ReadyButtonImages[Enum.GetValues(typeof(readyButtonStateEnum)).Length];

        _readyButtonStateValue[(int)readyButtonStateEnum.trueState].readyButtonImage = _button_Ready.GetComponent<Image>().sprite;
        _readyButtonStateValue[(int)readyButtonStateEnum.trueState].readyButtonText = _button_Ready.GetComponentInChildren<TMP_Text>().text;
        _readyButtonStateValue[(int)readyButtonStateEnum.trueState].readyButtonTextColor = _button_Ready.GetComponentInChildren<TMP_Text>().color;

        _readyButtonStateValue[(int)readyButtonStateEnum.cancelState].readyButtonImage = Managers.ResourceManager.Load<Sprite>("Art/UI/ButtonImage/Button_Rectangle_Red");
        _readyButtonStateValue[(int)readyButtonStateEnum.cancelState].readyButtonText = "Not Ready";
        _readyButtonStateValue[(int)readyButtonStateEnum.cancelState].readyButtonTextColor = Color.white;
    }

    protected override void OnEnableInit()
    {
        base.OnEnableInit();
        _loadingPanel.SetActive(false);
        _netWorkManager.OnClientConnectedCallback += EntetedPlayerinLobby;
        _netWorkManager.OnClientDisconnectCallback += DisConnetedPlayerinLobby;
        Managers.RelayManager.DisconnectPlayerAsyncEvent = Managers.LobbyManager.DisconnetPlayerinRoom;
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
            Managers.RelayManager.DisconnectPlayerAsyncEvent = null;
            await Managers.LobbyManager.TryJoinLobbyByNameOrCreateWaitLobby();
            Managers.SceneManagerEx.LoadScene(Define.Scene.LobbyScene);
            Debug.Log($"{Managers.LobbyManager.CurrentLobby.Name}");
           
        }
        catch (Exception error)
        {
            Debug.Log($"에러코드{error}");
        }

    }


    public void SetHostButton()
    {
        _button_Ready.gameObject.SetActive(false);
        _button_Start.gameObject.SetActive(true);
    }

    public void SetHostStartButton(bool startButtonstate)
    {
        _button_Start.interactable = startButtonstate;
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
            if (characterSelector.GetComponent<NetworkObject>().IsOwner)
            {
                _chracterSelectorNGO =  characterSelector.GetComponent<CharacterSelectorNGO>();    
            }
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
        //TODO: 크기 조절 되도록 수정
        GameObject characterSelecter = Managers.RelayManager.SpawnNetworkOBJ(playerIndex, characterSelector, Managers.RelayManager.NGO_ROOT_UI.transform,true);
        return characterSelecter;
    }
    public async Task LoadScenePlayGames()
    {
        // Managers.RelayManager.
        _netWorkManager.NetworkConfig.EnableSceneManagement = true;
        Managers.RelayManager.ChoicePlayerCharacter = (Define.PlayerClass)_chracterSelectorNGO.Module_ChooseCharacter_Move.PlayerChooseIndex;
        await Managers.SceneManagerEx.NetworkLoadSceneAsync(_netWorkManager, Define.Scene.GamePlayScene);
    }

    public void ButtonState(bool state)
    {
        _readyButtonState = state;
        ButtonImageChanged(state == false ? readyButtonStateEnum.trueState : readyButtonStateEnum.cancelState);
    }

    private void ButtonImageChanged(readyButtonStateEnum state)
    {
        ReadyButtonImages buttonimages = _readyButtonStateValue[(int)state];

        _button_Ready.image.sprite = buttonimages.readyButtonImage;
        _button_Text.text = buttonimages.readyButtonText;
        _button_Text.color = buttonimages.readyButtonTextColor;
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
