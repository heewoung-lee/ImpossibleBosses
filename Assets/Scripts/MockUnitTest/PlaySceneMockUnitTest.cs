using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Multiplayer.Playmode;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class PlaySceneMockUnitTest : MonoBehaviour
{
    public enum PlayersTag
    {
        Player1,
        Player2,
        Player3,
        Player4,
        None
    }
    
    string LobbyID = "TestLobby290";
    string _playerType = null;
    GameObject _ngoRoot;
    

    public Define.PlayerClass PlayerClass;
    public bool isSoloTest;
    private async void Start()
    {
       await JoinChannel();
    }
    private async Task JoinChannel()
    {
        Managers.RelayManager.ChoicePlayerCharacter = PlayerClass;
        if (Managers.RelayManager.NetWorkManager.IsListening == false)
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
                if (NetworkManager.Singleton.IsListening == true)
                {
                    Managers.RelayManager.Load_NGO_ROOT_UI_Module("NGO/PlayerSpawner");
                }
            }
            else
            {
                await Task.Delay(1000);
                (bool ischeckLobby, Lobby lobby) = await Managers.LobbyManager.TryGetLobbyAsyncCustom(LobbyID);
                if (ischeckLobby == false|| lobby.Data == null)
                {
                    await Utill.RateLimited(async() => await JoinChannel(),1000);
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
    public async void OnGUI()
    {
       if(GUI.Button(new Rect(0, 0, 100, 100), "GetLobby"))
        {

            (bool isgetLobby, Lobby lobby) = await Managers.LobbyManager.TryGetLobbyAsyncCustom(LobbyID);

            if(isgetLobby == false)
            {
                Debug.Log("로비가 존재하지 않습니다");
                return;
            }

            string joinCode = lobby.Data["RelayCode"].Value;
            Debug.Log($"조인코드: {joinCode}");
            foreach (NetworkClient player in Managers.RelayManager.NetWorkManager.ConnectedClientsList)
            {
                Debug.Log($"플레이어의 아이디: {player.ClientId}");
            }
        }
    }
}