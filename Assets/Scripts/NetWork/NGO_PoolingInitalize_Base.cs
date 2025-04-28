using UnityEngine;

public abstract class NGO_PoolingInitalize_Base : NGO_Skill_Initailize_Base
{
    public abstract string PoolingNGO_PATH { get; }
    public virtual void OnPoolGet() { }
    public virtual void OnPoolRelease() { }
    public abstract int PoolingCapacity { get; }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        PoolObjectSetParent();
    }
    private void PoolObjectSetParent()
    {
        if(Managers.NGO_PoolManager.Pool_NGO_Root_Dict.TryGetValue(PoolingNGO_PATH,out Transform parentTr))
        {
            transform.SetParent(parentTr,false);
        }
    }

}