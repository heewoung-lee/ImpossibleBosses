using System;
using System.Collections.Generic;
using Buffer;
using GameManagers.Interface;
using GameManagers.Interface.Resources_Interface;
using GameManagers.Interface.UI_Interface;
using Stats.BaseStats;
using UI.Scene.SceneUI;
using Unity.Netcode;
using UnityEngine;
using Util;
using Zenject;

namespace GameManagers
{
    public class BufferManager: IInitializable
    {
        
        
        
        private Dictionary<string, BuffModifier> _allBuffModifierDict = new Dictionary<string, BuffModifier>();
        private List<Type> _requestType = new List<Type>();
        private UIBufferBar _uiBufferBar;
        [Inject] private IUISceneManager _sceneManager;
        [Inject] IDestroyObject _destroyer;
        [Inject] private IInstantiate _instantiate;
        [Inject] DataManager _dataManager;
        public UIBufferBar UIBufferBar
        {
            get
            {
                if (_uiBufferBar == null)
                    _uiBufferBar = _sceneManager.Get_Scene_UI<UIBufferBar>();

                return _uiBufferBar;
            }
        }
        public BuffModifier GetModifier(StatEffect efftect)
        {
            return _allBuffModifierDict[efftect.buffname];
        }
        public BufferComponent InitBuff(BaseStats targetStat, float duration,StatEffect effect)
        {
            GameObject bufferGo = _instantiate.InstantiateByPath("Prefabs/Buffer/Buffer", UIBufferBar.BufferContext);
            BufferComponent buffer = _instantiate.GetOrAddComponent<BufferComponent>(bufferGo);
            buffer.InitAndStartBuff(targetStat, duration, effect);
            return buffer;
        }

        public BufferComponent InitBuff(BaseStats targetStat, float duration, BuffModifier bufferModifier,float value)
        {
            GameObject bufferGo = _instantiate.InstantiateByPath("Prefabs/Buffer/Buffer", UIBufferBar.BufferContext);
            BufferComponent buffer = _instantiate.GetOrAddComponent<BufferComponent>(bufferGo);
            buffer.InitAndStartBuff(targetStat, duration, bufferModifier, value);
            return buffer;
        }
        public void RemoveBuffer(BufferComponent buffer)
        {
            DurationBuff durationbuff = buffer.Modifier as DurationBuff;
            durationbuff.RemoveStats(buffer.TarGetStat, buffer.Value);
            _destroyer.DestroyObject(buffer.gameObject);
        }

        public void ImmediatelyBuffStart(BufferComponent buffer)
        {
            buffer.Modifier.ApplyStats(buffer.TarGetStat,buffer.Value);
            _destroyer.DestroyObject(buffer.gameObject);
        }

        public void Initialize()
        {
            _requestType = _dataManager.LoadSerializableTypesFromFolder("Assets/Scripts/Buffer/Buffer_Type", GetBuffModifierType);
            foreach(Type type in _requestType)
            {
                // Activator.CreateInstance로 인스턴스 생성 _requestType은 메타데이터 이므로 인스턴스가 아님
                //따라서 Type 메타정보를 바탕으로 인스턴스를 생성해줘야함

                BuffModifier modifierInstance = Activator.CreateInstance(type) as BuffModifier;
                _allBuffModifierDict.Add(modifierInstance.Buffname, modifierInstance);
            }
        }
        private void GetBuffModifierType(Type type, List<Type> typeList)
        {
            if (typeof(BuffModifier).IsAssignableFrom(type))
            {
                typeList.Add(type);
            }
        }

        public Collider[] DetectedPlayers()
        {
            LayerMask playerLayerMask = LayerMask.GetMask("Player") | LayerMask.GetMask("AnotherPlayer");
            float skillRadius = float.MaxValue;
            return  Physics.OverlapSphere(Vector3.zero,skillRadius,playerLayerMask);
        }
        public Collider[] DetectedOther(params string[] layerName)
        {
            LayerMask detectTargetMask = LayerMask.GetMask(layerName);
            float skillRadius = float.MaxValue;
            return  Physics.OverlapSphere(Vector3.zero,skillRadius,detectTargetMask);
        }

        public void ALL_Character_ApplyBuffAndCreateParticle(Collider[] targets,Action<NetworkObject> createPaticle,Action invokeBuff)
        {
            foreach (Collider playersCollider in targets)
            {
                if (playersCollider.TryGetComponent(out NetworkObject playerNgo))
                {
                    createPaticle.Invoke(playerNgo);
                    if (playerNgo.IsOwner)
                    {
                        invokeBuff.Invoke();
                    }

                }
            }
        }
    }
}