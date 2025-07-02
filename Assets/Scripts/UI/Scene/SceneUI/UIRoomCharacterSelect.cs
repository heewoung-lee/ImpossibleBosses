using System;
using System.Threading.Tasks;
using GameManagers;
using GameManagers.Interface;
using GameManagers.Interface.Resources_Interface;
using GameManagers.Interface.ResourcesManager;
using GameManagers.Interface.UIManager;
using Module.UI_Module;
using NetWork.NGO;
using Scene;
using Scene.GamePlayScene;
using TMPro;
using UI.SubItem;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Util;
using Zenject;

namespace UI.Scene.SceneUI
{
    struct ReadyButtonImages
    {
        public Sprite ReadyButtonImage;
        public string ReadyButtonText;
        public Color ReadyButtonTextColor;
    }

    public class UIRoomCharacterSelect : UIScene
    {
        [Inject] private IInstantiate _instantiate;
        [Inject] IResourcesLoader _resourcesLoader;
        [Inject] private LobbyManager _lobbyManager;
        [Inject] SceneManagerEx _sceneManagerEx;
        [Inject] private RelayManager _relayManager;
        [Inject] private IUISceneManager _uiSceneManager;
        [Inject] private IUISubItem _uiSubitem;
        [Inject] private CharacterSelectorNgo _characterSelectorNgo;
        
        private const int MaxPlayerCount = 8;

        enum ReadyButtonStateEnum
        {
            CancelState,
            TrueState
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
            ButtonReady,
            ButtonStart
        }
        private Transform _chooseCameraTr;
        private Transform _charactorSelect;
        private UIRoomPlayerFrame[] _uiRoomPlayerFrames;
        private Button _backToLobbyButton;
        private GameObject _loadingPanel;
        private UILoadingPanel _uiLoadingPanel;
        private GameObject _uiCharactorSelectRoot;
        private NetworkManager _netWorkManager;
        private Button _buttonReady;
        private Button _buttonStart;
        private TMP_Text _buttonText;
        //private CharacterSelectorNgo _characterSelectorNgo;
        private bool _readyButtonState;
        private Transform _ngoUIRootCharacterSelect;
        private string _joincodeCache;
        private Action _spawnCharacterSelectEvent;
        private ReadyButtonImages[] _readyButtonStateValue;

        
        

        public Transform ChooseCameraTr { get => _chooseCameraTr; }

        public event Action SpawnCharacterSelectEvent
        {
            add
            {
                UniqueEventRegister.AddSingleEvent(ref _spawnCharacterSelectEvent, value);
            }
            remove
            {
                UniqueEventRegister.RemovedEvent(ref _spawnCharacterSelectEvent, value);
            }
        }


        protected override void AwakeInit()
        {
            base.AwakeInit();
            Bind<Transform>(typeof(Transforms));
            Bind<Button>(typeof(Buttons));
            Bind<GameObject>(typeof(GameObjects));
            _chooseCameraTr = _instantiate.InstantiateByPath("Prefabs/Map/LobbyScene/ChoosePlayer").GetComponent<ModuleChooseCharactorTr>().ChooseCameraTr;
            _charactorSelect = Get<Transform>((int)Transforms.CharactorSelectTr);
            _backToLobbyButton = Get<Button>((int)Buttons.BackToLobbyButton);
            _buttonStart = Get<Button>((int)Buttons.ButtonStart);
            _buttonStart.onClick.AddListener(LoadScenePlayGames);
            _buttonStart.gameObject.SetActive(false);
            _backToLobbyButton.onClick.AddListener(async () =>
            {
                await BacktoLobby();
            });
            _loadingPanel = Get<GameObject>((int)GameObjects.LoadingPanel);

            _uiRoomPlayerFrames = new UIRoomPlayerFrame[MaxPlayerCount];
            for (int index = 0; index < _uiRoomPlayerFrames.Length; index++)
            {
                _uiRoomPlayerFrames[index] = _uiSubitem.MakeSubItem<UIRoomPlayerFrame>(_charactorSelect);
            }
            _netWorkManager = _relayManager.NetworkManagerEx;
            _buttonReady = Get<Button>((int)Buttons.ButtonReady);
            _buttonText = _buttonReady.GetComponentInChildren<TMP_Text>();
            ReadyButtonInitialize();
        }
        public void Set_NGO_UI_Root_Character_Select(Transform chracterRootTr)
        {
            _ngoUIRootCharacterSelect = chracterRootTr;
            _spawnCharacterSelectEvent?.Invoke();
        }
        private void ReadyButtonInitialize()
        {
            _readyButtonStateValue = new ReadyButtonImages[Enum.GetValues(typeof(ReadyButtonStateEnum)).Length];

            _readyButtonStateValue[(int)ReadyButtonStateEnum.TrueState].ReadyButtonImage = _buttonReady.GetComponent<Image>().sprite;
            _readyButtonStateValue[(int)ReadyButtonStateEnum.TrueState].ReadyButtonText = _buttonReady.GetComponentInChildren<TMP_Text>().text;
            _readyButtonStateValue[(int)ReadyButtonStateEnum.TrueState].ReadyButtonTextColor = _buttonReady.GetComponentInChildren<TMP_Text>().color;

            _readyButtonStateValue[(int)ReadyButtonStateEnum.CancelState].ReadyButtonImage = _resourcesLoader.Load<Sprite>("Art/UI/ButtonImage/Button_Rectangle_Red");
            _readyButtonStateValue[(int)ReadyButtonStateEnum.CancelState].ReadyButtonText = "Not Ready";
            _readyButtonStateValue[(int)ReadyButtonStateEnum.CancelState].ReadyButtonTextColor = Color.white;
        }

        protected override void OnEnableInit()
        {
            base.OnEnableInit();
            _loadingPanel.SetActive(false);
        }

        private void SubScribeRelayCallback()
        {
            _netWorkManager.OnClientConnectedCallback += EntetedPlayerinLobby;
            _netWorkManager.OnClientDisconnectCallback += DisConnetedPlayerinLobby;
        }
        private void UnscribeRelayCallback()
        {
            _netWorkManager.OnClientConnectedCallback -= EntetedPlayerinLobby;
            _netWorkManager.OnClientDisconnectCallback -= DisConnetedPlayerinLobby;
        }
        public void SetButtonEvent(UnityAction action)
        {
            _buttonReady.onClick.AddListener(action);
        }

        private void DisConnetedPlayerinLobby(ulong playerIndex)
        {
            Debug.Log("플레이어가 나갔습니다.");
            IsCheckAllReadyToPlayers(playerIndex);
        }
        public void IsCheckAllReadyToPlayers(ulong playerIndex = ulong.MaxValue)
        {
            foreach (CharacterSelectorNgo playerNgo in _relayManager.NgoRootUI.GetComponentsInChildren<CharacterSelectorNgo>())
            {
                if (playerNgo.GetComponent<NetworkObject>().OwnerClientId == playerIndex)
                    continue;

                if (playerNgo.IsOwnedByServer)
                    continue;

                if (playerNgo.IsReady == false)
                {
                    SetHostStartButton(false);
                    return;
                }
            }
            SetHostStartButton(true);
        }
        public void EntetedPlayerinLobby(ulong playerIndex)
        {
            Debug.Log("EnteredPlayerinLobby 이벤트 발생");
            SetHostStartButton(false);
            SpawnChractorSeletorAndSetPosition(playerIndex);
        }

        public async Task BacktoLobby()
        {
            try
            {
                _loadingPanel.SetActive(true);
                UnscribeRelayCallback();
                Lobby currentLobby = await _lobbyManager.GetCurrentLobby();
                _lobbyManager.HostChageEvent -= OnHostMigrationEvent;
                await _lobbyManager.TryJoinLobbyByNameOrCreateWaitLobby();
                _sceneManagerEx.LoadScene(Define.Scene.LobbyScene);
            }
            catch (Exception error)
            {
                Debug.Log($"에러코드{error}");
            }
        }

        public void SetHostButton()
        {
            _buttonReady.gameObject.SetActive(false);
            _buttonStart.gameObject.SetActive(true);
        }

        public void SetHostStartButton(bool startButtonstate)
        {
            _buttonStart.interactable = startButtonstate;
        }
        protected override void StartInit()
        {
            base.StartInit();
            _uiLoadingPanel = _uiSceneManager.GetSceneUIFromResource<UILoadingPanel>();
            InitializeCharacterSelectionAsHost();
            _lobbyManager.HostChageEvent += OnHostMigrationEvent;
        }

        private void OnHostMigrationEvent()
        {
            InitializeCharacterSelectionAsHost();
        }

        private void InitializeCharacterSelectionAsHost()
        {
            if (_relayManager.NetworkManagerEx.IsHost == false)
                return;
            _relayManager.SpawnToRPC_Caller();
            _relayManager.SpawnNetworkObj("Prefabs/NGO/NGOUIRootChracterSelect", parent: _relayManager.NgoRootUI.transform);
            SpawnChractorSeletorAndSetPosition(_netWorkManager.LocalClientId);
            SubScribeRelayCallback();
        }
        private void SpawnChractorSeletorAndSetPosition(ulong playerIndex)
        {
            if (_netWorkManager.IsHost)
            {
                GameObject characterSelector = _characterSelectorNgo.gameObject;
                SetPositionCharacterSelector(characterSelector, playerIndex);
            }
        }



        private GameObject SetPositionCharacterSelector(GameObject characterSelector, ulong playerIndex)
        {
            int playerframeindex = 0;
            for (int i = 0; i < _uiRoomPlayerFrames.Length; i++)
            {
                if (_uiRoomPlayerFrames[i].CharacterSelectorNgo == null)
                {
                    _uiRoomPlayerFrames[i].SetCharacterSelector(characterSelector);
                    playerframeindex = i;
                    break;
                }
            }

            GameObject targetFrame = _uiRoomPlayerFrames[playerframeindex].gameObject;
            RectTransform targetFrameRect = targetFrame.GetComponent<RectTransform>();

            Vector2 frameSize = GetUISize(targetFrame);
            Vector2 frameScreenPos = GetUIScreenPosition(targetFrameRect);

            GameObject characterRect =
                _relayManager.SpawnNetworkOBJInjectionOnwer(playerIndex, characterSelector, frameScreenPos, destroyOption: true);

            if (_ngoUIRootCharacterSelect == null)
            {
                SpawnCharacterSelectEvent += SetParentPosition;
            }
            else
            {
                characterSelector.transform.SetParent(_ngoUIRootCharacterSelect, worldPositionStays: false);
            }
            //TODO: 크기 조절 되도록 수정
            return characterSelector;



            void SetParentPosition()
            {
                characterSelector.transform.SetParent(_ngoUIRootCharacterSelect, worldPositionStays: false);
            }

        }
        public void LoadScenePlayGames()//호스트가 Start버튼을 클릭했을때
        {
            _netWorkManager.NetworkConfig.EnableSceneManagement = true;
            _relayManager.RegisterSelectedCharacter(_relayManager.NetworkManagerEx.LocalClientId, (Define.PlayerClass)_characterSelectorNgo.ModuleChooseCharacterMove.PlayerChooseIndex);
            _sceneManagerEx.OnClientLoadedEvent += ClientLoadedEvent;
            _sceneManagerEx.OnAllPlayerLoadedEvent += AllPlayerLoadedEvent;
            _sceneManagerEx.NetworkLoadScene(Define.Scene.GamePlayScene);

            void ClientLoadedEvent(ulong clientId)
            {
                _relayManager.NgoRPCCaller.GetPlayerChoiceCharacterRpc(clientId);
                Debug.Log(_sceneManagerEx.GetCurrentScene.CurrentScene + "씬네임" + "플레이어 ID" + clientId);
            }

            void AllPlayerLoadedEvent()
            {
                PlayScene playScene = null;
                foreach (BaseScene scene in _sceneManagerEx.GetCurrentScenes)
                {
                    if (scene is PlayScene outPlayScene)
                    {
                        playScene = outPlayScene;
                        break;
                    }
                }
            }
        }

        public void ButtonState(bool state)
        {
            _readyButtonState = state;
            ButtonImageChanged(state == false ? ReadyButtonStateEnum.TrueState : ReadyButtonStateEnum.CancelState);
        }

        private void ButtonImageChanged(ReadyButtonStateEnum state)
        {
            ReadyButtonImages buttonimages = _readyButtonStateValue[(int)state];

            _buttonReady.image.sprite = buttonimages.ReadyButtonImage;
            _buttonText.text = buttonimages.ReadyButtonText;
            _buttonText.color = buttonimages.ReadyButtonTextColor;
        }

    }
}