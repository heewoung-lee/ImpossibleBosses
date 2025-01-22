using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public enum SelectDirection
{
    LeftClick,
    RightClick
}
public class UI_RoomPlayerFrame : UI_Base
{
    private readonly Color PLAYER_FRAME_COLOR = "#143658".HexCodetoConvertColor();
    private readonly Color EMPTY_PLAYER_FRAME_COLOR = "#988B8B50".HexCodetoConvertColor();

    enum Images
    {
        Bg
    }
    enum GameObjects
    {
        Character,
        NickNamePanel,
        ReadyPanel,
        HostMaker
    }

    enum Buttons
    {
        PreviousPlayerBTN,
        NextPlayerBTN,
    }


    private Button _previousButton;
    private Button _nextButton;
    private Module_ChoosePlayer_Manager _charactor_Selector;
    private GameObject _selectCharacterObject;
    private GameObject _playerNickNameObject;
    private GameObject _readyPanel;
    private GameObject _hostMarker;
    private Image _bg;
    protected override void AwakeInit()
    {
        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));
        Bind<Image>(typeof(Images));
        _previousButton = Get<Button>((int)Buttons.PreviousPlayerBTN);
        _nextButton = Get<Button>((int)Buttons.NextPlayerBTN);
        _selectCharacterObject = Get<GameObject>((int)GameObjects.Character);
        _playerNickNameObject = Get<GameObject>((int)GameObjects.NickNamePanel);
        _readyPanel = Get<GameObject>((int)GameObjects.ReadyPanel);
        _hostMarker = Get<GameObject>((int)GameObjects.HostMaker);
        _bg = Get<Image>((int)Images.Bg);
        _bg.color = EMPTY_PLAYER_FRAME_COLOR;
    }

    protected override void StartInit()
    {
        _charactor_Selector = Managers.UI_Manager.Get_Scene_UI<UI_Room_CharacterSelect>().PlayerSelector.GetComponent<Module_ChoosePlayer_Manager>();
    }


    private void EnterPlayerSetting()
    {
        _bg.color = PLAYER_FRAME_COLOR;
        _selectCharacterObject.SetActive(true);
        _playerNickNameObject.SetActive(true);
        _previousButton.onClick.AddListener(() => _charactor_Selector.MoveSelectCamera(SelectDirection.LeftClick));
        _nextButton.onClick.AddListener(() => _charactor_Selector.MoveSelectCamera(SelectDirection.RightClick));
    }


}
