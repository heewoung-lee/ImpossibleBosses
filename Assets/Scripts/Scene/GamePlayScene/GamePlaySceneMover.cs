using GameManagers;
using NetWork.NGO;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

namespace Scene.GamePlayScene
{
    public class GamePlaySceneMover : ISceneMover
    {
        public void MoveScene()
        {
            if (Managers.RelayManager.NetworkManagerEx.IsHost == false)
                return;

            Managers.RelayManager.NetworkManagerEx.NetworkConfig.EnableSceneManagement = true;
            Managers.SceneManagerEx.OnAllPlayerLoadedEvent += SetPlayerPostion;
            Managers.SceneManagerEx.NetworkLoadScene(Define.Scene.GamePlayScene);
            Managers.RelayManager.NgoRPCCaller.ResetManagersRpc();

            void SetPlayerPostion()
            {
                foreach (NetworkObject player in Managers.RelayManager.NetworkManagerEx.SpawnManager.SpawnedObjectsList)
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
