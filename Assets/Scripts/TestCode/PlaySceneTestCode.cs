using System;
using System.Threading.Tasks;
using Unity.Multiplayer.Playmode;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay.Models;
using UnityEngine;

public class PlaySceneTestCode : MonoBehaviour
{
    public enum PlayersTag
    {
        Player1,
        Player2,
        Player3,
        Player4,
        None
    }

    string LobbyID = "TestLobby27";
    string _playerType = null;
    private async void Start()
    {
        await JoinChannel();
    }

    private async Task JoinChannel()
    {
        if (Managers.RelayManager.NetWorkManager.IsListening == false)
        {
            await SetAuthenticationService();
            if (_playerType == "Player1")
            {
                await Managers.LobbyManager.CreateLobbyID(LobbyID, "TestLobby", 8);
            }
            else
            {
                (bool ischeckLobby, Lobby lobby) = await Managers.LobbyManager.TryGetLobbyAsyncCustom(LobbyID);
                if (ischeckLobby == false)
                {
                    await Utill.RateLimited(async() => await JoinChannel(),5000);
                }
               await Managers.LobbyManager.JoinLobbyByID(LobbyID);
            }

        }
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



    public async void OnGUI()
    {
       if(GUI.Button(new Rect(0, 0, 100, 100), "GetLobby"))
        {
          Lobby lobby = await Managers.LobbyManager.GetLobbyAsyncCustom(LobbyID);
            if(lobby == null)
            {
                Debug.Log(lobby + "�� �����Ͱ� �����ϴ�");
                return;
            }

            Debug.Log($"{lobby.Id}�� �÷��̾� ���");
            Debug.Log($"�����ڵ�{lobby.Data["RelayCode"].Value}");
            foreach (Player player in lobby.Players)
            {
                Debug.Log($"���� �����ϰ� �ִ� �÷��̾� : {player.Id}");
            }

        }
    }
}