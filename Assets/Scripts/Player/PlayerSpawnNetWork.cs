using Unity.Netcode;
using UnityEngine;

public class PlayerSpawnNetWork : NetworkBehaviourBase
{
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsOwner)
        {
            SetTransformServerRpc();
            Managers.SocketEventManager.PlayerSpawnInitalize?.Invoke(gameObject);
        }

    }
    protected override void AwakeInit()
    {
    }

    protected override void StartInit()
    {
    }


    [ServerRpc]
    private void SetTransformServerRpc()
    {
        transform.SetParent(Managers.RelayManager.NGO_ROOT.transform);
    }

}