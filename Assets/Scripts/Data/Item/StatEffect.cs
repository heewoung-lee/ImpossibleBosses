using System;
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

    public StatType statType;  // ��ȭ ����
    public float value;        // ��ȭ ��
    public string buffname;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref statType);
        serializer.SerializeValue(ref value);
        serializer.SerializeValue(ref buffname);
    }
}
