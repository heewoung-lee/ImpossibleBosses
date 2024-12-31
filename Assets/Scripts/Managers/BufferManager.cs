using BehaviorDesigner.Runtime.Tactical.Tasks;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


public class BufferManager:IManagerInitializable
{

    Dictionary<string, Buff_Modifier> _allBuffModifierDict = new Dictionary<string, Buff_Modifier>();

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

    public Buff_Modifier GetModifier(ItemEffect efftect)
    {
        return _allBuffModifierDict[efftect.buffname];
    }

    
    public Buffer InitBuff(BaseStats targetStat, float duration,ItemEffect effect)
    {
        Buffer buffer = Managers.ResourceManager.InstantiatePrefab("Buffer/Buffer", UI_BufferBar.BufferContext).GetOrAddComponent<Buffer>();
        buffer.InitAndStartBuff(targetStat, duration, effect);
        return buffer;
    }
    public void RemoveBuffer(Buffer buffer)
    {
      Duration_Buff durationbuff = buffer.Modifier as Duration_Buff;
      durationbuff.RemoveStats(buffer.TarGetStat, buffer.Value);
      Managers.ResourceManager.DestroyObject(buffer.gameObject);
    }

    public void ImmediatelyBuffStart(Buffer buffer)
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

            Buff_Modifier modifierInstance = Activator.CreateInstance(type) as Buff_Modifier;

            _allBuffModifierDict.Add(modifierInstance.Buffname, modifierInstance);
        }
    }
    private void GetBuffModifierType(Type type, List<Type> typeList)
    {
        if (typeof(Buff_Modifier).IsAssignableFrom(type))
        {
            typeList.Add(type);
        }
    }
}