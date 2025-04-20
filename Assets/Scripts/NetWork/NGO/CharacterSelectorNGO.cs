using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.UI;
using static Define;
public class CharacterSelectorNGO : NetworkBehaviourBase
{
    private readonly Color PLAYER_FRAME_COLOR = "#143658".HexCodetoConvertColor();

    private NetworkVariable<FixedString64Bytes> _playerNickname = new NetworkVariable<FixedString64Bytes>(
    new FixedString64Bytes(""), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);// 서버만 수정 가능하도록 설정

    private NetworkVariable<bool> _isReady = new NetworkVariable<bool>(
    false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private NetworkVariable<Vector3> _characterSeletorCamera = new NetworkVariable<Vector3>(
    Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public enum CameraOperation
    {
        Set,
        Add
    }

    enum RawImages
    {
        SelectPlayerRawImage
    }
    enum Cameras
    {
        SelectCamara
    }
    enum Images
    {
        Bg,
        HostIMage
    }
    enum GameObjects
    {
        NickNamePanel,
        ReadyPanel,
    }

    enum Buttons
    {
        PreviousPlayerBTN,
        NextPlayerBTN,
    }


    private Image _bg;
    private Image _hostIMage;
    private Button _previousButton;
    private Button _nextButton;
    private GameObject _playerNickNameObject;
    private GameObject _readyPanel;
    private TMP_Text _playerNickNameText;
    private UI_Room_CharacterSelect _ui_Room_CharacterSelect;
    private Camera _playerChooseCamera;
    private RawImage _selectPlayerRawImage;
    private bool isRunnningCoroutine = false;
    private Coroutine _cameraMoveCoroutine;
    private bool _isInitCameraPosition = false;
    private Module_ChooseCharacter_Move _module_chooseCharacter_Move;

    public Button PreViousButton { get => _previousButton; }
    public Button NextButton { get => _nextButton; }
    public bool ISReady { get => _isReady.Value; }
    public RawImage SelectPlayerRawImage
    {
        get
        {
            if (_selectPlayerRawImage == null)
            {
                _selectPlayerRawImage = Get<RawImage>((int)RawImages.SelectPlayerRawImage);
            }
            return _selectPlayerRawImage;
        }
    }
    public Module_ChooseCharacter_Move Module_ChooseCharacter_Move
    {
        get
        {
            if (_module_chooseCharacter_Move == null)
            {
                _module_chooseCharacter_Move = GetComponent<Module_ChooseCharacter_Move>();
            }
            return _module_chooseCharacter_Move;
        }
    }
    protected override void AwakeInit()
    {
        Bind<Image>(typeof(Images));
        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));
        Bind<Camera>(typeof(Cameras));
        Bind<RawImage>(typeof(RawImages));

        _bg = Get<Image>((int)Images.Bg);
        _hostIMage = Get<Image>((int)Images.HostIMage);
        _previousButton = Get<Button>((int)Buttons.PreviousPlayerBTN);
        _nextButton = Get<Button>((int)Buttons.NextPlayerBTN);
        _playerNickNameObject = Get<GameObject>((int)GameObjects.NickNamePanel);
        _readyPanel = Get<GameObject>((int)GameObjects.ReadyPanel);
        _playerChooseCamera = Get<Camera>((int)Cameras.SelectCamara);

        _hostIMage.gameObject.SetActive(false);
        SetActiveCharacterSelectionArrow(false);
        _readyPanel.gameObject.SetActive(false);

        _playerNickNameText = _playerNickNameObject.GetComponentInChildren<TMP_Text>();
        _ui_Room_CharacterSelect = Managers.UI_Manager.Get_Scene_UI<UI_Room_CharacterSelect>();
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        _bg.color = PLAYER_FRAME_COLOR;
        if (IsOwner)
        {
            _previousButton.gameObject.SetActive(true);
            _nextButton.gameObject.SetActive(true);
            _ui_Room_CharacterSelect.ButtonState(false);
            SetNicknameServerRpc(Managers.LobbyManager.CurrentPlayerInfo.PlayerNickName);
            _ui_Room_CharacterSelect.SetButtonEvent(() => PlayerReadyServerRpc());
            SetPositionCharacterChooseCamera();
        }
        if (IsHost && IsOwner)//호스트 최초 1번 호출부
        {
            _ui_Room_CharacterSelect.SetHostButton();

            Managers.RelayManager.NetworkManagerEx.OnClientDisconnectCallback -= CheckHostIsAlone;
            Managers.RelayManager.NetworkManagerEx.OnClientDisconnectCallback += CheckHostIsAlone;
            _ui_Room_CharacterSelect.SetHostStartButton(true);
        }
        DisPlayHostMarker();
        _playerChooseCamera.transform.localPosition = _characterSeletorCamera.Value;
        _characterSeletorCamera.OnValueChanged += SelecterCameraOnValueChanged;
        _playerNickname.OnValueChanged += (oldValue, newValue) =>
        {
            _playerNickNameText.text = newValue.ToString();
        };
        _isReady.OnValueChanged += (oldValue, newValue) =>
        {
            _readyPanel.SetActive(newValue);
            if (IsOwner)
            {
                SetActiveCharacterSelectionArrow(!newValue);
                Define.PlayerClass selectCharacter = (Define.PlayerClass)Module_ChooseCharacter_Move.PlayerChooseIndex;
                Managers.RelayManager.RegisterSelectedCharacter(Managers.RelayManager.NetworkManagerEx.LocalClientId, selectCharacter);
            }
        };
        // UI 초기화
        _playerNickNameText.text = _playerNickname.Value.ToString();
        _readyPanel.SetActive(_isReady.Value);
    }

    private void CheckHostIsAlone(ulong clientId)
    {
        if (IsHost == false)
            return;

        if (Managers.RelayManager.NetworkManagerEx.ConnectedClientsIds.Count == 1)
        {
            _ui_Room_CharacterSelect.SetHostStartButton(true);
        }
        else
        {
            _ui_Room_CharacterSelect.SetHostStartButton(false);
        }
    }


    public void SelecterCameraOnValueChanged(Vector3 oldValue, Vector3 newValue)
    {
        if (_isInitCameraPosition == false)
        {
            _playerChooseCamera.transform.localPosition = newValue;
            _isInitCameraPosition = true;
            return;
        }
        if (IsOwner)
        {
            if (isRunnningCoroutine == true)
            {
                StopCoroutine(_cameraMoveCoroutine);
                _playerChooseCamera.transform.localPosition = oldValue;
            }
            _cameraMoveCoroutine = StartCoroutine(MoveCameraLinear(newValue));
        }
        else
        {
            _playerChooseCamera.transform.localPosition = newValue;
        }
    }
    private void SetActiveCharacterSelectionArrow(bool State)
    {
        _previousButton.gameObject.SetActive(State);
        _nextButton.gameObject.SetActive(State);
    }
    private void SetPositionCharacterChooseCamera()
    {
        Vector3 targetWorldPosition = _ui_Room_CharacterSelect.ChooseCameraTr.position;
        Vector3 targetLocalPosition = transform.InverseTransformPoint(targetWorldPosition);
        SetCameraPositionServerRpc(targetLocalPosition, CameraOperation.Set);
    }
    [Rpc(SendTo.Server)]
    public void SetCameraPositionServerRpc(Vector3 position, CameraOperation cameraOperation, RpcParams rpcParams = default)
    {
        switch (cameraOperation)
        {
            case CameraOperation.Set: 
                _characterSeletorCamera.Value = position;
                break;
            case CameraOperation.Add:
                _characterSeletorCamera.Value += position;
                break;
        }
    }
    [Rpc(SendTo.Server)]
    private void SetNicknameServerRpc(string newNickname, RpcParams rpcParams = default)
    {
        _playerNickname.Value = new FixedString64Bytes(newNickname);
    }
    [Rpc(SendTo.Server)]
    public void PlayerReadyServerRpc(RpcParams rpcParams = default)
    {
        _isReady.Value = !_isReady.Value;
        _readyPanel.SetActive(_isReady.Value);

        isCheckReadyToPlayers();

        NotifyButtonStateClientRpc(_isReady.Value, rpcParams.Receive.SenderClientId);
    }
    [Rpc(SendTo.NotMe)]
    private void NotifyButtonStateClientRpc(bool state, ulong targetClientId)
    {
        if (NetworkManager.Singleton.LocalClientId == targetClientId)
        {
            _ui_Room_CharacterSelect.ButtonState(state); // 본인의 클라이언트에서만 실행
        }
    }
    private void isCheckReadyToPlayers()
    {
        foreach (CharacterSelectorNGO playerNGO in Managers.RelayManager.NGO_ROOT_UI.GetComponentsInChildren<CharacterSelectorNGO>())
        {
            if (playerNGO.IsOwnedByServer)
                continue;

            if (playerNGO.ISReady == false)
            {
                _ui_Room_CharacterSelect.SetHostStartButton(false);
                return;
            }
        }
        _ui_Room_CharacterSelect.SetHostStartButton(true);
    }



    private void DisPlayHostMarker()
    {
        if (IsOwnedByServer)
        {
            _hostIMage.gameObject.SetActive(true);
        }
    }
    protected override void StartInit()
    {
    }
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
    }
    public void SetSelectPlayerRawImage(Texture texture)
    {
        SelectPlayerRawImage.texture = texture;
    }
    IEnumerator MoveCameraLinear(Vector3 moveDirection)
    {
        float elapseTime = 0f;
        float DurationTime = 1f;
        isRunnningCoroutine = true;
        while (elapseTime < DurationTime)
        {
            elapseTime += Time.deltaTime;
            _playerChooseCamera.transform.localPosition = Vector3.Lerp(_playerChooseCamera.transform.localPosition, moveDirection, elapseTime);
            yield return null;
        }
        isRunnningCoroutine = false;
        _playerChooseCamera.transform.localPosition = moveDirection;
    }
}
