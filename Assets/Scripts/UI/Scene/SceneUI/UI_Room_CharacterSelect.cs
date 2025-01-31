using System;
using System.Threading.Tasks;
using Unity.Services.Lobbies;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_Room_CharacterSelect : UI_Scene
{
    private const int MAX_PLAYER_COUNT = 8;
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

    }
    GameObject _playerSelector;
    public GameObject PlayerSelector { get { return _playerSelector; } }
    
    Transform _charactorSelectTr;

    UI_RoomPlayerFrame[] _ui_RoomPlayerFrames;
    Button _backToLobbyButton;
    GameObject _loadingPanel;

    protected override void AwakeInit()
    {
        base.AwakeInit();
        Bind<Transform>(typeof(Transforms));
        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));
        _playerSelector = Managers.ResourceManager.InstantiatePrefab("Map/ChoosePlayer");
        _charactorSelectTr = Get<Transform>((int)Transforms.CharactorSelectTr);
        _backToLobbyButton = Get<Button>((int)Buttons.BackToLobbyButton);
        _backToLobbyButton.onClick.AddListener(async () =>
        {
            await BacktoLobby();
        });
        _loadingPanel = Get<GameObject>((int)GameObjects.LoadingPanel);
        
        _ui_RoomPlayerFrames = new UI_RoomPlayerFrame[MAX_PLAYER_COUNT];
        for (int index = 0; index < _ui_RoomPlayerFrames.Length; index++)
        {
            _ui_RoomPlayerFrames[index] = Managers.UI_Manager.MakeSubItem<UI_RoomPlayerFrame>(_charactorSelectTr);
        }
    }

    protected override void OnEnableInit()
    {
        base.OnEnableInit();
        _loadingPanel.SetActive(false);
    }

    

    public async Task BacktoLobby()
    {
        try
        {
            _loadingPanel.SetActive(true);
            await Managers.LobbyManager.TryJoinLobbyByNameOrCreateWaitLobby();
            Managers.SceneManagerEx.LoadScene(Define.Scene.LobbyScene);
        }
        catch(Exception error)
        {
            Debug.Log($"에러코드{error}");
        }

    }
}
