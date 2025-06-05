using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.AI;

public class UnitPlayScenePlayerPosition : IPlayerPositionController
{
    public void SetPlayerPosition()
    {
        if (Managers.GameManagerEx.Player == null)
        {
            Managers.GameManagerEx.OnPlayerSpawnEvent += (PlayerStats) => { PlayerSpawnPosition(PlayerStats.GetComponent<NavMeshAgent>()); };
        }
        else
        {
            PlayerSpawnPosition(Managers.GameManagerEx.Player.GetComponent<NavMeshAgent>());
        }
    }
    public void PlayerSpawnPosition(NavMeshAgent navMesh)
    {
        Vector3 spawnPos = new Vector3(Managers.GameManagerEx.Player.GetComponent<NetworkObject>().OwnerClientId, 0, 0);

        foreach(var ngo in Managers.RelayManager.NetworkManagerEx.SpawnManager.SpawnedObjectsList)
        {
            if (ngo.IsOwner == false)
                continue;

           if( ngo.TryGetComponent(out NavMeshAgent navMash))
            {
                Debug.Log(navMash.GetInstanceID() + "현재 캐릭의 인스턴스 ID");
            }

        }


        Debug.Log($"{navMesh.GetInstanceID()}유저 스폰 포지션{spawnPos}");
        navMesh.Warp(spawnPos);

        navMesh.transform.position = spawnPos;
        navMesh.GetComponent<NetworkTransform>().transform.position = spawnPos;
    }
}