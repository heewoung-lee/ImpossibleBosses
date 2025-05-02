using System;
using UnityEngine;
[RequireComponent(typeof(Poolable))]
public abstract class NGO_PoolingInitalize_Base : NGO_Skill_Initailize_Base
{
    private Action _poolObjectReleaseEvent;
    private Action _poolObjectGetEvent;

    public event Action PoolObjectReleaseEvent
    {
        add
        {
            UniqueEventRegister.AddSingleEvent(ref _poolObjectReleaseEvent,value);
        }
        remove
        {
            UniqueEventRegister.RemovedEvent(ref _poolObjectReleaseEvent, value);

        }
    }
    public event Action PoolObjectGetEvent
    {
        add
        {
            UniqueEventRegister.AddSingleEvent(ref _poolObjectGetEvent,value);
        }
        remove
        {
            UniqueEventRegister.RemovedEvent(ref _poolObjectGetEvent, value);
        }
    }

    public abstract string PoolingNGO_PATH { get; }
    public virtual void OnPoolGet()
    {
        _poolObjectGetEvent?.Invoke();
    }
    public virtual void OnPoolRelease()
    {
        _poolObjectReleaseEvent?.Invoke();
    }
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