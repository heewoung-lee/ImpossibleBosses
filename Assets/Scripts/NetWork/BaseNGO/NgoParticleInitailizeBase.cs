using System;
using GameManagers;
using Unity.Netcode;
using UnityEngine;

namespace NetWork.BaseNGO
{
    public abstract class NgoParticleInitailizeBase : NgoInitailizeBase
    {
        public abstract NetworkObject TargetNgo { get; }
        public abstract void SetTargetInitalze(NetworkObject targetNgo);

        public virtual void StartParticle(string path,float duration, Action<GameObject> positionAndBehaviorSetterEvent)
        {
            Managers.VFXManager.SetPariclePosAndLifeCycle(ParticleNgo.gameObject, path, duration, positionAndBehaviorSetterEvent);
        }
    }
}
