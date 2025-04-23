using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Multiplayer.Playmode;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class PlaySceneMockUnitTest : BaseScene
{
    public enum PlayersTag
    {
        Player1,
        Player2,
        Player3,
        Player4,
        None
    }
    
    string LobbyID = "TestLobby322";
    string _playerType = null;
    GameObject _ngoRoot;

    private UI_Loading _ui_Loading_Scene;
    public Define.PlayerClass PlayerClass;
    public bool isSoloTest;

    public override Define.Scene CurrentScene => Define.Scene.GamePlayScene;

    protected override async void StartInit()
    {
        base.StartInit();
        _ui_Loading_Scene = Managers.UI_Manager.GetOrCreateSceneUI<UI_Loading>();
        await JoinChannel();
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
                if (NetworkManager.Singleton.IsListening == true)
                {
                    Init_NGO_PlayScene_OnHost();
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
        LoadGamePlayScene();
        Managers.LobbyManager.InitalizeLobbyEvent();
        Managers.LobbyManager.SubScribeRelayCallBack();
    }

    private void LoadGamePlayScene()
    {
        _ui_Loading_Scene.gameObject.SetActive(false);
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
    }

    private void Init_NGO_PlayScene_OnHost()
    {
        if (Managers.RelayManager.NetworkManagerEx.IsHost)
        {
            Managers.RelayManager.Load_NGO_Prefab<NGO_PlaySceneSpawn>();
            Managers.NGO_PoolManager.Create_NGO_Pooling_Object();//네트워크 오브젝트 풀링 생성
        }
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


    //public async void OnGUI()
    //{
    //   if(GUI.Button(new Rect(0, 0, 100, 100), "GetLobby"))
    //    {

    //        (bool isgetLobby, Lobby lobby) = await Managers.LobbyManager.TryGetLobbyAsyncCustom(LobbyID);

    //        if(isgetLobby == false)
    //        {
    //            Debug.Log("로비가 존재하지 않습니다");
    //            return;
    //        }

    //        string joinCode = lobby.Data["RelayCode"].Value;
    //        Debug.Log($"조인코드: {joinCode}");
    //        foreach (NetworkClient player in Managers.RelayManager.NetworkManagerEx.ConnectedClientsList)
    //        {
    //            Debug.Log($"플레이어의 아이디: {player.ClientId}");
    //        }
    //    }
    //}
}