using BehaviorDesigner.Runtime.Tactical.Tasks;
using Mono.Cecil;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Buffer;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class BufferManager:IManagerInitializable
{

    Dictionary<string, BuffModifier> _allBuffModifierDict = new Dictionary<string, BuffModifier>();
    List<Type> _requestType = new List<Type>();

    UI_BufferBar _ui_BufferBar;

    public UI_BufferBar UI_BufferBar
    {
        get
        {
            if (_ui_BufferBar == null)
                _ui_BufferBar = Managers.UI_Manager.Get_Scene_UI<UI_BufferBar>();

            return _ui_BufferBar;
        }
    }
    public BuffModifier GetModifier(StatEffect efftect)
    {
        return _allBuffModifierDict[efftect.buffname];
    }
    public BufferComponent InitBuff(BaseStats targetStat, float duration,StatEffect effect)
    {
        BufferComponent buffer = Managers.ResourceManager.Instantiate("Prefabs/Buffer/Buffer", UI_BufferBar.BufferContext).GetOrAddComponent<BufferComponent>();
        buffer.InitAndStartBuff(targetStat, duration, effect);
        return buffer;
    }

    public BufferComponent InitBuff(BaseStats targetStat, float duration, BuffModifier buffer_modifier,float value)
    {
        BufferComponent buffer = Managers.ResourceManager.Instantiate("Prefabs/Buffer/Buffer", UI_BufferBar.BufferContext).GetOrAddComponent<BufferComponent>();
        buffer.InitAndStartBuff(targetStat, duration, buffer_modifier, value);
        return buffer;
    }
    public void RemoveBuffer(BufferComponent buffer)
    {
      DurationBuff durationbuff = buffer.Modifier as DurationBuff;
      durationbuff.RemoveStats(buffer.TarGetStat, buffer.Value);
      Managers.ResourceManager.DestroyObject(buffer.gameObject);
    }

    public void ImmediatelyBuffStart(BufferComponent buffer)
    {
        buffer.Modifier.ApplyStats(buffer.TarGetStat,buffer.Value);
        Managers.ResourceManager.DestroyObject(buffer.gameObject);
    }

    public void Init()
    {
        _requestType = Managers.DataManager.LoadSerializableTypesFromFolder("Assets/Scripts/Buffer/Buffer_Type", GetBuffModifierType);
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
        return Physics.OverlapSphere(Vector3.zero, skillRadius, playerLayerMask);
    }
    public Collider[] DetectedOther(params string[] layerName)
    {
        LayerMask detectTargetMask = LayerMask.GetMask(layerName);
        float skillRadius = float.MaxValue;
        return Physics.OverlapSphere(Vector3.zero, skillRadius, detectTargetMask);
    }

    public void ALL_Character_ApplyBuffAndCreateParticle(Collider[] targets,Action<NetworkObject> createPaticle,Action invokeBuff)
    {
        foreach (Collider players_collider in targets)
        {
            if (players_collider.TryGetComponent(out NetworkObject playerNGO))
            {
                createPaticle.Invoke(playerNGO);
                if (playerNGO.IsOwner)
                {
                    invokeBuff.Invoke();
                }

            }
        }
    }
}