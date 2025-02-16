using System;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UI_Room_Info_Panel : UI_Base
{

    enum Texts
    {
        RoomNameText,
        CurrentCount
    }
    enum Buttons { JoinButton }



    private TMP_Text _roomNameText;
    private TMP_Text _currentPlayerCount;
    private Button _joinButton;

    private Lobby _lobbyRegisteredPanel;
    public Lobby LobbyRegisteredPanel => _lobbyRegisteredPanel;
    protected override void AwakeInit()
    {
        Bind<TMP_Text>(typeof(Texts));
        Bind<Button>(typeof(Buttons));
        _roomNameText = Get<TMP_Text>((int)Texts.RoomNameText);
        _currentPlayerCount = Get<TMP_Text>((int)Texts.CurrentCount);
        _joinButton = Get<Button>((int)Buttons.JoinButton);
        _joinButton.onClick.AddListener(async ()=>await AddJoinEvent());
        _joinButton.onClick.AddListener(() =>
        {
            _joinButton.interactable = false;
        });
    }

    protected override void StartInit()
    {
    }

    public void SetRoomInfo(Lobby lobby)
    {
        _lobbyRegisteredPanel = lobby;
        _roomNameText.text = lobby.Name;
        _currentPlayerCount.text = $"{lobby.Players.Count}/{lobby.MaxPlayers}";
    }


    public async Task AddJoinEvent()
    {
        if (_lobbyRegisteredPanel.HasPassword)
        {
            UI_InputRoomPassWord ui_inputPassword = Managers.UI_Manager.TryGetPopupDictAndShowPopup<UI_InputRoomPassWord>();
            ui_inputPassword.SetRoomInfoPanel(this);
        }
        else
        {
            try
            {
                await Managers.LobbyManager.JoinLobbyByID(_lobbyRegisteredPanel.Id);
                Managers.SceneManagerEx.LoadScene(Define.Scene.RoomScene);
            }
            catch (LobbyServiceException notFoundLobby) when(notFoundLobby.Message.Contains("lobby not found")) 
            {
                string errorMsg = "방이 없습니다.";
                Debug.Log($"{errorMsg}");
                Managers.UI_Manager.TryGetPopupDictAndShowPopup<UI_AlertDialog>()
                    .AlertSetText("오류",$"{errorMsg}")
                    .AfterAlertEvent(async() =>
                    {
                        await Managers.LobbyManager.ReFreshRoomList();
                    });
                _joinButton.interactable = true;
                return;
            }
        }
    }

}
