using System;
using Unity.Netcode;
using UnityEngine;
[RequireComponent(typeof(Poolable))]
public abstract class NGO_PoolingInitalize_Base : NGO_Particle_Initailize_Base
{
    private NetworkObject _particleNGO;
    private NetworkObject _targetNGO;

    public override NetworkObject TargetNgo => _targetNGO;
    public override NetworkObject ParticleNGO => _particleNGO;

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

    public override void SetInitalze(NetworkObject particleOBJ)
    {
        _particleNGO = particleOBJ;
    }
    public override void SetTargetInitalze(NetworkObject targetNgo)
    {
        _targetNGO = targetNgo;
    }
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
    public override void StartParticle(string path, float duration, Action<GameObject> positionAndBehaviorSetterEvent)
    {
        base.StartParticle(path, duration, positionAndBehaviorSetterEvent);
        StartParticleOption();
    }
    
    protected virtual void StartParticleOption(){}
}