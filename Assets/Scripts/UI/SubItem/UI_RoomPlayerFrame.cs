using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class UI_RoomPlayerFrame : UI_Base
{
    enum Images
    {
        Bg
    }
    private readonly Color EMPTY_PLAYER_FRAME_COLOR = "#988B8B50".HexCodetoConvertColor();
    private Module_ChoosePlayer_Manager _charactor_Selector;
    private CharacterSelectorNGO _characterNgo;
    private Image _bg;
    public CharacterSelectorNGO ChracterSelectorNGO { get => _characterNgo; }

    protected override void AwakeInit()
    {
        Bind<Image>(typeof(Images));
        _bg = Get<Image>((int)Images.Bg);
        _bg.color = EMPTY_PLAYER_FRAME_COLOR;
    }

    protected override void StartInit()
    {
        _charactor_Selector = Managers.UI_Manager.Get_Scene_UI<UI_Room_CharacterSelect>().PlayerSelector.GetComponent<Module_ChoosePlayer_Manager>();
    }

    public void SetCharacterSelector(GameObject chracterSelecter)
    {
        _characterNgo = chracterSelecter.GetComponent<CharacterSelectorNGO>();
    }


    
}
