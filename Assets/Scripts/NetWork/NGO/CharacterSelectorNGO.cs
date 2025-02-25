using System;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
public enum SelecterDirection
{
    LeftClick,
    RightClick
}
public class CharacterSelectorNGO : NetworkBehaviourBase
{
    private readonly Color PLAYER_FRAME_COLOR = "#143658".HexCodetoConvertColor();

    private NetworkVariable<FixedString64Bytes> _playerNickname = new NetworkVariable<FixedString64Bytes>(
    new FixedString64Bytes(""),NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Server );// 서버만 수정 가능하도록 설정
    
    private NetworkVariable<bool> _isReady = new NetworkVariable<bool>(
    false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private NetworkVariable<Vector3> _characterSeletorCamera = new NetworkVariable<Vector3>(
    Vector3.zero,NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);


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

    protected override void AwakeInit()
    {
        Bind<Image>(typeof(Images));
        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));
        _bg = Get<Image>((int)Images.Bg);
        _hostIMage = Get<Image>((int)Images.HostIMage);
        _previousButton = Get<Button>((int)Buttons.PreviousPlayerBTN);
        _nextButton = Get<Button>((int)Buttons.NextPlayerBTN);
        _playerNickNameObject = Get<GameObject>((int)GameObjects.NickNamePanel);
        _readyPanel = Get<GameObject>((int)GameObjects.ReadyPanel);

        _hostIMage.gameObject.SetActive(false);
        _previousButton.gameObject.SetActive(false);
        _nextButton.gameObject.SetActive(false);
        _readyPanel.gameObject.SetActive(false);

        _playerNickNameText = _playerNickNameObject.GetComponentInChildren<TMP_Text>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        _bg.color = PLAYER_FRAME_COLOR;
        _playerNickNameObject.SetActive(true);
        if (IsOwner)
        {
            _previousButton.gameObject.SetActive(true);
            _nextButton.gameObject.SetActive(true);
            _ui_Room_CharacterSelect = Managers.UI_Manager.Get_Scene_UI<UI_Room_CharacterSelect>();
            _ui_Room_CharacterSelect.ButtonState(false);
            Debug.Log($"오너의 클라이언트ID{Managers.LobbyManager.CurrentPlayerInfo.PlayerNickName}");
            SetNicknameServerRpc(Managers.LobbyManager.CurrentPlayerInfo.PlayerNickName);
            Managers.UI_Manager.Get_Scene_UI<UI_Room_CharacterSelect>().SetButtonEvent(()=> PlayerReadyServerRpc());
        }
        DisPlayHostMarker();
        _playerNickname.OnValueChanged += (oldValue, newValue) => {
            _playerNickNameText.text = newValue.ToString();
        };
        _isReady.OnValueChanged += (oldValue, newValue) =>
        {
            _readyPanel.SetActive(newValue);
        };
        // UI 초기화
        _playerNickNameText.text = _playerNickname.Value.ToString();
        _readyPanel.SetActive(_isReady.Value);
    }

    [ServerRpc]
    private void SetNicknameServerRpc(string newNickname, ServerRpcParams rpcParams = default)
    {
       _playerNickname.Value = new FixedString64Bytes(newNickname);
    }
    [ServerRpc]
    public void PlayerReadyServerRpc(ServerRpcParams rpcParams = default)
    {
        _isReady.Value = !_isReady.Value;
        _readyPanel.SetActive(_isReady.Value);

        NotifyButtonStateClientRpc(_isReady.Value, rpcParams.Receive.SenderClientId);
    }
    [ClientRpc]
    private void NotifyButtonStateClientRpc(bool state, ulong targetClientId)
    {
        if (NetworkManager.Singleton.LocalClientId == targetClientId)
        {
            _ui_Room_CharacterSelect.ButtonState(state); // 본인의 클라이언트에서만 실행
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
    }

   
}
