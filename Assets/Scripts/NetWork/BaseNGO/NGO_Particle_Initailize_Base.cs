using System;
using GameManagers;
using Unity.Netcode;
using UnityEngine;

public abstract class NGO_Particle_Initailize_Base : NGO_InitailizeBase
{
    public abstract NetworkObject TargetNgo { get; }
    public abstract void SetTargetInitalze(NetworkObject targetNgo);

    public virtual void StartParticle(string path,float duration, Action<GameObject> positionAndBehaviorSetterEvent)
    {
        Managers.VFXManager.SetPariclePosAndLifeCycle(ParticleNGO.gameObject, path, duration, positionAndBehaviorSetterEvent);
    }
}
