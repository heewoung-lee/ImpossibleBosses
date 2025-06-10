using System;
using Data.DataType.ItemType.Interface;
using Unity.Netcode;
using UnityEngine;

[Serializable]
public struct StatEffect : INetworkSerializable
{
    public StatEffect(StatType statType, float value, string buffname)
    {
        this.statType = statType;
        this.value = value;
        this.buffname = buffname;
    }

    public StatType statType;  // 변화 스탯
    public float value;        // 변화 값
    public string buffname;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref statType);
        serializer.SerializeValue(ref value);
        serializer.SerializeValue(ref buffname);
    }
}
