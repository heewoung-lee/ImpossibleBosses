using Unity.VisualScripting;
using UnityEngine;

public class UI_Room_CharacterSelect : UI_Scene
{
    private const int MAX_PLAYER_COUNT = 8;
    enum Transforms
    {
        CharactorSelectTr
    }
    GameObject _playerSelector;
    public GameObject PlayerSelector { get { return _playerSelector; } }
    
    Transform _charactorSelectTr;

    UI_RoomPlayerFrame[] _ui_RoomPlayerFrames;


    protected override void AwakeInit()
    {
        base.AwakeInit();
        Bind<Transform>(typeof(Transforms));
        _playerSelector = Managers.ResourceManager.InstantiatePrefab("Map/ChoosePlayer");
        _charactorSelectTr = Get<Transform>((int)Transforms.CharactorSelectTr);

        _ui_RoomPlayerFrames = new UI_RoomPlayerFrame[MAX_PLAYER_COUNT];

        for(int index = 0; index < _ui_RoomPlayerFrames.Length; index++)
        {
            _ui_RoomPlayerFrames[index] = Managers.UI_Manager.MakeSubItem<UI_RoomPlayerFrame>(_charactorSelectTr);
        }


    }
}
