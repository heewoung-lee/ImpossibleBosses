using GameManagers;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class UnitBattleScenePlayerPosition : IPlayerPositionController
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
    void PlayerSpawnPosition(NavMeshAgent navMesh)
    {
        navMesh.Warp(new Vector3(Managers.GameManagerEx.Player.GetComponent<NetworkObject>().OwnerClientId, 0, 0));
    }
}