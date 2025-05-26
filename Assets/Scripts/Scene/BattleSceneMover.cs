using System;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using static BattleSceneMover;

public class BattleSceneMover : ISceneMover
{
    public void MoveScene()
    {
        if (Managers.RelayManager.NetworkManagerEx.IsHost == false)
            return;

        Managers.RelayManager.NGO_RPC_Caller.ResetManagersRpc();
        Managers.RelayManager.NetworkManagerEx.NetworkConfig.EnableSceneManagement = true;

        Managers.SceneManagerEx.NetworkLoadScene(Define.Scene.BattleScene, ClientLoadedEvent, () => { });
        void ClientLoadedEvent(ulong clientId)
        {
            Debug.Log($"{clientId} 플레이어 로딩 완료");

            foreach (NetworkObject clicentNgoObj in Managers.RelayManager.NetworkManagerEx.SpawnManager.SpawnedObjectsList)
            {
                if (clicentNgoObj.OwnerClientId != clientId)
                {
                    continue;
                }
                if (clicentNgoObj.TryGetComponent(out PlayerStats playerStats) == true)
                {
                    Debug.Log($"{clientId}플레이어 찾았다");
                    playerStats.transform.SetParent(Managers.RelayManager.NGO_ROOT.transform);
                    playerStats.transform.position = new Vector3(clientId, 0, 0);
                    break;
                }
            }
        }
    }
}
