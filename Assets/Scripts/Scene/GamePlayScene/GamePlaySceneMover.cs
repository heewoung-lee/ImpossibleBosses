using GameManagers;
using NetWork.NGO;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using Util;
using Zenject;

namespace Scene.GamePlayScene
{
    public class GamePlaySceneMover : ISceneMover
    {
        [Inject] SceneManagerEx _sceneManagerEx;
        [Inject] private RelayManager _relayManager;

        public void MoveScene()
        {
            if (_relayManager.NetworkManagerEx.IsHost == false)
                return;

            _relayManager.NetworkManagerEx.NetworkConfig.EnableSceneManagement = true;
            _sceneManagerEx.OnAllPlayerLoadedEvent += SetPlayerPostion;
            _sceneManagerEx.NetworkLoadScene(Define.Scene.GamePlayScene);
            _relayManager.NgoRPCCaller.ResetManagersRpc();

            void SetPlayerPostion()
            {
                foreach (NetworkObject player in _relayManager.NetworkManagerEx.SpawnManager.SpawnedObjectsList)
                {
                    Vector3 pos = new Vector3(player.OwnerClientId, 0, 0);

                    if (player.TryGetComponent(out NavMeshAgent agent))
                    {
                        agent.Warp(pos);
                        player.GetComponent<PlayerInitializeNgo>().SetForcePositionFromNetworkRpc(pos);
                    }

                }
            }
        }
    }
}
