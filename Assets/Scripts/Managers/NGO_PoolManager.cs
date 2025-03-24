using Unity.Netcode;
using Unity.VisualScripting;

public class NGO_PoolManager
{
    private NetworkObjectPool _ngoPool;

    public NetworkObjectPool NgoPool
    {
        get
        {
            if(_ngoPool == null)
            {
                Managers.RelayManager.NGO_RPC_Caller.SpawnPrefabNeedToInitalize("NGO/NGO_Polling");
            }
            return _ngoPool;    
        }
    }

    public void Set_NGO_Pool(NetworkObject ngo)
    {
        _ngoPool = ngo.gameObject.GetComponent<NetworkObjectPool>();
    }

}