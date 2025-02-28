using Unity.Netcode;

public class PlayerSpawnNetWork : NetworkBehaviourBase
{
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsOwner)
        {
            SetTransformServerRpc();
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