using System;
using System.Threading.Tasks;
using GameManagers;
using Unity.Multiplayer.Playmode;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class MockUnitBattleScene : ISceneSpawnBehaviour
{
    public MockUnitBattleScene(Define.PlayerClass playerClass, UI_Loading ui_Loading, bool isSoloTest)
    {
        _playerClass = playerClass;
        _ui_Loading_Scene = ui_Loading;
        _isSoloTest = isSoloTest;
    }
    public enum PlayersTag
    {
        Player1,
        Player2,
        Player3,
        Player4,
        None
    }
    string _playerType = null;
    GameObject _ngoRoot;
    private const string LobbyName = "TestLobby";
    private UI_Loading _ui_Loading_Scene;
    private Define.PlayerClass _playerClass;
    private bool _isSoloTest;

    public ISceneMover nextscene => new GamePlaySceneMover();

    public async void Init()
    {
        await JoinChannel();
    }
    public void SpawnOBJ()
    {
        if (Managers.RelayManager.NetworkManagerEx.IsListening)
        {
            Init_NGO_BattleScene_OnHost();
        }
        Managers.RelayManager.NetworkManagerEx.OnServerStarted += Init_NGO_BattleScene_OnHost;
        void Init_NGO_BattleScene_OnHost()
        {
            if (Managers.RelayManager.NetworkManagerEx.IsHost)
            {
                Managers.RelayManager.Load_NGO_Prefab<NGO_BattleSceneSpawn>();
                Managers.NGO_PoolManager.Create_NGO_Pooling_Object();
            }
        }
    }
    private async Task JoinChannel()
    {
        Managers.RelayManager.NetworkManagerEx.OnClientConnectedCallback -= ConnectClicent;
        Managers.RelayManager.NetworkManagerEx.OnClientConnectedCallback += ConnectClicent;
        if (Managers.RelayManager.NetworkManagerEx.IsListening == false)
        {
            await SetAuthenticationService();
            if (_playerType == "Player1")
            {
                if (_isSoloTest == true)//나혼자 테스트 할때
                {
                    await Managers.RelayManager.StartHostWithRelay(8);
                }
                else
                {
                    await Managers.LobbyManager.CreateLobby("TestLobby", 8, null);
                }
            }
            else
            {

                await Task.Delay(1000);
                Lobby lobby = await Managers.LobbyManager.AvailableLobby(LobbyName);
                if (lobby == null || lobby.Data == null)
                {
                    await Utill.RateLimited(async () => await JoinChannel(), 1000);
                    return;
                }
                string joinCode = lobby.Data["RelayCode"].Value;
                await Managers.RelayManager.JoinGuestRelay(joinCode);
            }
        }
    }
    private void ConnectClicent(ulong clientID)
    {
        if (Managers.RelayManager.NGO_RPC_Caller == null)
        {
            Managers.RelayManager.Spawn_RpcCaller_Event += SpawnPlayer;
        }
        else
        {
            SpawnPlayer();
        }
        void SpawnPlayer()
        {
            if (Managers.RelayManager.NetworkManagerEx.LocalClientId != clientID)
                return;
            Define.PlayerClass playerClass =
                (int)_playerClass + (int)clientID < Enum.GetValues(typeof(Define.PlayerClass)).Length
                ? (Define.PlayerClass)((int)_playerClass + (int)clientID) : Define.PlayerClass.Archer;
            Managers.RelayManager.RegisterSelectedCharacter(clientID, playerClass);
            Managers.RelayManager.NGO_RPC_Caller.GetPlayerChoiceCharacterRpc(clientID);
            LoadBattleScene();
        }
    }

    private void LoadBattleScene()
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
}
