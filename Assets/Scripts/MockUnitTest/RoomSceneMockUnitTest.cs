using System;
using System.Threading.Tasks;
using Unity.Multiplayer.Playmode;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class RoomSceneMockUnitTest : BaseScene
{
    public enum PlayersTag
    {
        Player1,
        Player2,
        Player3,
        Player4,
        None
    }

    string LobbyID = "TestLobby05";
    string _playerType = null;
    GameObject _ngoRoot;

    public bool isSoloTest;

    public override Define.Scene CurrentScene => Define.Scene.RoomScene;



    protected override async void StartInit()
    {
        base.StartInit();
        await JoinChannel();
        Managers.RelayManager.SpawnToRPC_Caller();
        UI_Room_CharacterSelect uI_CharacterSelect = Managers.UI_Manager.GetSceneUIFromResource<UI_Room_CharacterSelect>();
        UI_RoomChat ui_Chatting = Managers.UI_Manager.GetSceneUIFromResource<UI_RoomChat>();
    }

    private async Task JoinChannel()
    {
        if (Managers.RelayManager.NetworkManagerEx.IsListening == false)
        {
            await SetAuthenticationService();
            if (_playerType == "Player1")
            {
                if (isSoloTest == true)//나혼자 테스트 할때
                {
                    await Managers.RelayManager.StartHostWithRelay(8);
                }
                else
                {
                    await Managers.LobbyManager.CreateLobbyID(LobbyID, "TestLobby", 8);
                }
            }
            else
            {
                await Task.Delay(1000);
                (bool ischeckLobby, Lobby lobby) = await Managers.LobbyManager.TryGetLobbyAsyncCustom(LobbyID);
                if (ischeckLobby == false || lobby.Data == null)
                {
                    await Utill.RateLimited(async () => await JoinChannel(), 1000);
                    return;
                }
                string joinCode = lobby.Data["RelayCode"].Value;
                await Managers.RelayManager.JoinGuestRelay(joinCode);
            }
        }
        Managers.LobbyManager.InitalizeLobbyEvent();
        Managers.LobbyManager.InitalizeRelayEvent();
    }

    private async Task SetAuthenticationService()
    {

        _playerType = GetPlayerTag();
        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        string playerID = AuthenticationService.Instance.PlayerId;
        Managers.LobbyManager.SetPlayerLoginInfo(new PlayerIngameLoginInfo(_playerType, playerID));
        Managers.SocketEventManager.DisconnectApiEvent += Managers.LobbyManager.LogoutAndAllLeaveLobby;
    }


    public string GetPlayerTag()
    {
        string[] tagValue = CurrentPlayer.ReadOnlyTags();

        PlayersTag currentPlayer = PlayersTag.Player1;
        if (tagValue.Length > 0 && Enum.TryParse(typeof(PlayersTag), tagValue[0], out var parsedEnum))
        {
            currentPlayer = (PlayersTag)parsedEnum;
        }
        return Enum.GetName(typeof(PlayersTag), currentPlayer);
    }

    protected override void AwakeInit()
    {
    }

    public override void Clear()
    {
    }
}
