using System;
using Buffer;
using Stats.BaseStats;
using UI.Scene.SceneUI;
using Unity.Netcode;
using UnityEngine;

namespace GameManagers.Interface.BufferManager
{
    public interface IBufferManager
    {
        public BuffModifier GetModifier(StatEffect efftect);
        public BufferComponent InitBuff(BaseStats targetStat, float duration, StatEffect effect);
        public BufferComponent InitBuff(BaseStats targetStat, float duration, BuffModifier bufferModifier, float value);
        public void RemoveBuffer(BufferComponent buffer);
        public void ImmediatelyBuffStart(BufferComponent buffer);

        public void ALL_Character_ApplyBuffAndCreateParticle(Collider[] targets, Action<NetworkObject> createPaticle,
            Action invokeBuff);

    }
}
