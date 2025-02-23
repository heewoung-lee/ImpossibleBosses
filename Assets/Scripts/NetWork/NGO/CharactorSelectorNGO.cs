using System.Runtime.CompilerServices;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
public enum SelecterDirection
{
    LeftClick,
    RightClick
}
public class CharactorSelectorNGO : NetworkBehaviourBase
{
    private readonly Color PLAYER_FRAME_COLOR = "#143658".HexCodetoConvertColor();
    private readonly Color EMPTY_PLAYER_FRAME_COLOR = "#988B8B50".HexCodetoConvertColor();

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
        _bg.color = EMPTY_PLAYER_FRAME_COLOR;

        _hostIMage.gameObject.SetActive(false);
        _previousButton.gameObject.SetActive(false);
        _nextButton.gameObject.SetActive(false);
        _readyPanel.gameObject.SetActive(false);

        _playerNickNameText = _playerNickNameObject.GetComponentInChildren<TMP_Text>();
        _playerNickNameText.text = Managers.LobbyManager.CurrentPlayerInfo.PlayerNickName;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        _bg.color = PLAYER_FRAME_COLOR;
        _playerNickNameObject.SetActive(true);
        if (IsOwner)
        {
            if (IsHost)
            {
                _hostIMage.gameObject.SetActive(true);
            }
            _previousButton.gameObject.SetActive(true);
            _nextButton.gameObject.SetActive(true);
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
