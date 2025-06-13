using System.Collections;
using GameManagers;
using Module.UI_Module;
using NetWork.BaseNGO;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;


namespace NetWork.NGO
{
public class CharacterSelectorNgo : NetworkBehaviourBase
{
    private readonly Color _playerFrameColor = "#143658".HexCodetoConvertColor();

    private NetworkVariable<FixedString64Bytes> _playerNickname = new NetworkVariable<FixedString64Bytes>(
        new FixedString64Bytes(""), NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server); // 서버만 수정 가능하도록 설정

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
        PreviousPlayerBtn,
        NextPlayerBtn,
    }


    private Image _bg;
    private Image _hostIMage;
    private Button _previousButton;
    private Button _nextButton;
    private GameObject _playerNickNameObject;
    private GameObject _readyPanel;
    private TMP_Text _playerNickNameText;
    private UI_Room_CharacterSelect _uiRoomCharacterSelect;
    private Camera _playerChooseCamera;
    private RawImage _selectPlayerRawImage;
    private bool _isRunnningCoroutine = false;
    private Coroutine _cameraMoveCoroutine;
    private bool _isInitCameraPosition = false;
    private ModuleChooseCharacterMove _moduleChooseCharacterMove;

    public Button PreViousButton
    {
        get => _previousButton;
    }

    public Button NextButton
    {
        get => _nextButton;
    }

    public bool IsReady
    {
        get => _isReady.Value;
    }

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

    public ModuleChooseCharacterMove ModuleChooseCharacterMove
    {
        get
        {
            if (_moduleChooseCharacterMove == null)
            {
                _moduleChooseCharacterMove = GetComponent<ModuleChooseCharacterMove>();
            }

            return _moduleChooseCharacterMove;
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
        _previousButton = Get<Button>((int)Buttons.PreviousPlayerBtn);
        _nextButton = Get<Button>((int)Buttons.NextPlayerBtn);
        _playerNickNameObject = Get<GameObject>((int)GameObjects.NickNamePanel);
        _readyPanel = Get<GameObject>((int)GameObjects.ReadyPanel);
        _playerChooseCamera = Get<Camera>((int)Cameras.SelectCamara);

        _hostIMage.gameObject.SetActive(false);
        SetActiveCharacterSelectionArrow(false);
        _readyPanel.gameObject.SetActive(false);

        _playerNickNameText = _playerNickNameObject.GetComponentInChildren<TMP_Text>();
        _uiRoomCharacterSelect = Managers.UIManager.Get_Scene_UI<UI_Room_CharacterSelect>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        _bg.color = _playerFrameColor;
        if (IsOwner)
        {
            _previousButton.gameObject.SetActive(true);
            _nextButton.gameObject.SetActive(true);
            _uiRoomCharacterSelect.ButtonState(false);
            SetNicknameServerRpc(Managers.LobbyManager.CurrentPlayerInfo.PlayerNickName);
            _uiRoomCharacterSelect.SetButtonEvent(() => PlayerReadyServerRpc());
            SetPositionCharacterChooseCamera();
        }

        if (IsHost && IsOwner) //호스트 최초 1번 호출부
        {
            _uiRoomCharacterSelect.SetHostButton();

            Managers.RelayManager.NetworkManagerEx.OnClientDisconnectCallback -= CheckHostIsAlone;
            Managers.RelayManager.NetworkManagerEx.OnClientDisconnectCallback += CheckHostIsAlone;
            _uiRoomCharacterSelect.SetHostStartButton(true);
        }

        DisPlayHostMarker();
        _playerChooseCamera.transform.localPosition = _characterSeletorCamera.Value;
        _characterSeletorCamera.OnValueChanged += SelecterCameraOnValueChanged;
        _playerNickname.OnValueChanged += (oldValue, newValue) => { _playerNickNameText.text = newValue.ToString(); };
        _isReady.OnValueChanged += (oldValue, newValue) =>
        {
            _readyPanel.SetActive(newValue);
            if (IsOwner)
            {
                SetActiveCharacterSelectionArrow(!newValue);
                Define.PlayerClass selectCharacter = (Define.PlayerClass)ModuleChooseCharacterMove.PlayerChooseIndex;
                Managers.RelayManager.RegisterSelectedCharacter(Managers.RelayManager.NetworkManagerEx.LocalClientId,
                    selectCharacter);
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
            _uiRoomCharacterSelect.SetHostStartButton(true);
        }
        else
        {
            _uiRoomCharacterSelect.SetHostStartButton(false);
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
            if (_isRunnningCoroutine == true)
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

    private void SetActiveCharacterSelectionArrow(bool state)
    {
        _previousButton.gameObject.SetActive(state);
        _nextButton.gameObject.SetActive(state);
    }

    private void SetPositionCharacterChooseCamera()
    {
        Vector3 targetWorldPosition = _uiRoomCharacterSelect.ChooseCameraTr.position;
        Vector3 targetLocalPosition = transform.InverseTransformPoint(targetWorldPosition);
        SetCameraPositionServerRpc(targetLocalPosition, CameraOperation.Set);
    }

    [Rpc(SendTo.Server)]
    public void SetCameraPositionServerRpc(Vector3 position, CameraOperation cameraOperation,
        RpcParams rpcParams = default)
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
        _uiRoomCharacterSelect.isCheckAllReadyToPlayers();
        NotifyButtonStateClientRpc(_isReady.Value, rpcParams.Receive.SenderClientId);
    }

    [Rpc(SendTo.NotMe)]
    private void NotifyButtonStateClientRpc(bool state, ulong targetClientId)
    {
        if (Managers.RelayManager.NetworkManagerEx.LocalClientId == targetClientId)
        {
            _uiRoomCharacterSelect.ButtonState(state); // 본인의 클라이언트에서만 실행
        }
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
        Managers.RelayManager.NetworkManagerEx.OnClientDisconnectCallback -= CheckHostIsAlone;
    }

    public void SetSelectPlayerRawImage(Texture texture)
    {
        SelectPlayerRawImage.texture = texture;
    }

    IEnumerator MoveCameraLinear(Vector3 moveDirection)
    {
        float elapseTime = 0f;
        float durationTime = 1f;
        _isRunnningCoroutine = true;
        while (elapseTime < durationTime)
        {
            elapseTime += Time.deltaTime;
            _playerChooseCamera.transform.localPosition = Vector3.Lerp(_playerChooseCamera.transform.localPosition,
                moveDirection, elapseTime);
            yield return null;
        }

        _isRunnningCoroutine = false;
        _playerChooseCamera.transform.localPosition = moveDirection;
    }
}
}