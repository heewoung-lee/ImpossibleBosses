using System;
using GameManagers;
using Unity.Netcode;
using UnityEngine;

namespace NetWork.BaseNGO
{
    [RequireComponent(typeof(Poolable))]
    public abstract class NgoPoolingInitalizeBase : NgoParticleInitailizeBase
    {
        private NetworkObject _particleNgo;
        private NetworkObject _targetNgo;

        public override NetworkObject TargetNgo => _targetNgo;
        public override NetworkObject ParticleNgo => _particleNgo;

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
        public abstract string PoolingNgoPath { get; }

        public override void SetInitalze(NetworkObject particleObj)
        {
            _particleNgo = particleObj;
        }
        public override void SetTargetInitalze(NetworkObject targetNgo)
        {
            _targetNgo = targetNgo;
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
            if(Managers.NgoPoolManager.PoolNgoRootDict.TryGetValue(PoolingNgoPath,out Transform parentTr))
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
}