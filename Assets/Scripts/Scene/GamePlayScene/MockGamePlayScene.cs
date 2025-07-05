using System;
using System.Threading.Tasks;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityTransform;
using GameManagers;
using GameManagers.Interface.LoginManager;
using GameManagers.Interface.UIManager;
using NetWork.NGO;
using NetWork.NGO.UI;
using Scene.BattleScene;
using Scene.CommonInstaller;
using UI.Scene.SceneUI;
using Unity.Multiplayer.Playmode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using Util;
using Zenject;
using Object = UnityEngine.Object;

namespace Scene.GamePlayScene
{
    public class MockGamePlayScene : ISceneSpawnBehaviour
    {
        [Inject] private LobbyManager _lobbyManager;
        [Inject] private RelayManager _relayManager;
        [Inject] private IUISceneManager _uisceneManager;
        [Inject] private IFactory<NgoGamePlaySceneSpawn> _ngogamePlaySceneSpawnFactory;

        private Define.PlayerClass _playerClass;
        private NgoGamePlaySceneSpawn _ngoGamePlaySceneSpawn;
        
        public void Init()
        {
            _playerClass = (Object.FindAnyObjectByType<PlayScene>() as ISceneSelectCharacter).GetPlayerableCharacter();
            _relayManager.NetworkManagerEx.OnClientConnectedCallback -= ConnectClicent;
            _relayManager.NetworkManagerEx.OnClientConnectedCallback += ConnectClicent;
        }
        private void ConnectClicent(ulong clientID)
        {
            if (_relayManager.NgoRPCCaller == null)
            {
                _relayManager.SpawnRpcCallerEvent += SpawnPlayer;
            }
            else
            {
                SpawnPlayer();
            }
            void SpawnPlayer()
            {
                if (_relayManager.NetworkManagerEx.LocalClientId != clientID)
                    return;
               
                _relayManager.RegisterSelectedCharacter(clientID, _playerClass);
                _relayManager.NgoRPCCaller.GetPlayerChoiceCharacterRpc(clientID);
                LoadGamePlayScene();
            }
        }
        private void LoadGamePlayScene()
        {
            _uisceneManager.GetOrCreateSceneUI<UILoading>().gameObject.SetActive(false);
        }
        public void SpawnObj()
        {
            if (_relayManager.NetworkManagerEx.IsListening)
            {
                InitNgoPlaySceneOnHost();
            }
            _relayManager.NetworkManagerEx.OnServerStarted += InitNgoPlaySceneOnHost;
            void InitNgoPlaySceneOnHost()
            {
                if (_relayManager.NetworkManagerEx.IsHost)
                {
                    _ngoGamePlaySceneSpawn = _ngogamePlaySceneSpawnFactory.Create();
                    _relayManager.SpawnNetworkObj(_ngoGamePlaySceneSpawn.gameObject, _relayManager.NgoRoot.transform);
                }
            }
        }
    }
}
