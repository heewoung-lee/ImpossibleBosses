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
    new FixedString64Bytes(""), // �⺻���� �� ���ڿ��� ����
    NetworkVariableReadPermission.Everyone,
    NetworkVariableWritePermission.Server // ������ ���� �����ϵ��� ����
);

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

            Debug.Log($"������ Ŭ���̾�ƮID{Managers.LobbyManager.CurrentPlayerInfo.PlayerNickName}");
            SetNicknameServerRpc(Managers.LobbyManager.CurrentPlayerInfo.PlayerNickName);
        }
        DisPlayHostMarker();
        _playerNickname.OnValueChanged += (oldValue, newValue) => {
            _playerNickNameText.text = newValue.ToString();
        };

        // UI �ʱ�ȭ
        _playerNickNameText.text = _playerNickname.Value.ToString();
    }

    [ServerRpc]
    private void SetNicknameServerRpc(string newNickname, ServerRpcParams rpcParams = default)
    {
        // ��û�� Ŭ���̾�Ʈ�� ID�� ������
        ulong clientId = rpcParams.Receive.SenderClientId;

        // �ش� Ŭ���̾�Ʈ�� ������ ������Ʈ�� NetworkVariable�� ����
        if (OwnerClientId == clientId) // ��û�� Ŭ���̾�Ʈ�� �� ������Ʈ�� �������� ��쿡�� ����
        {
            _playerNickname.Value = new FixedString64Bytes(newNickname);
            Debug.Log($"�÷��̾� {clientId}�� �г����� '{newNickname}'�� ������");
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
