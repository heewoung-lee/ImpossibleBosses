using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;


public struct SpawnParamBase : INetworkSerializable
{
    public float argFloat;
    public Vector3 argPosVector3;
    public Vector3 argEulerAnglesVector3;
    public FixedString512Bytes argString;
    public int argInteger;
    public ulong argUlong;
    public bool argBoolean;
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref argFloat);
        serializer.SerializeValue(ref argPosVector3);
        serializer.SerializeValue(ref argEulerAnglesVector3);
        serializer.SerializeValue(ref argString);
        serializer.SerializeValue(ref argInteger);
        serializer.SerializeValue(ref argUlong);
        serializer.SerializeValue(ref argBoolean);
    }
    public static SpawnParamBase Create(float? argFloat = null, Vector3? argPosVector3 = null,Vector3? argEulerAnglesVector3 = null, string argString = null,
        int? argInteger = null,ulong? argUlong = null,bool? argBoolean = null)
    {
        return new SpawnParamBase
        {
            argPosVector3 = argPosVector3 ?? Vector3.zero,
            argEulerAnglesVector3 = argEulerAnglesVector3 ?? Vector3.zero,
            argString = argString == null ? default : new FixedString512Bytes(argString),
            argFloat = argFloat ?? 0f,
            argInteger = argInteger ?? 0,
            argUlong = argUlong ?? 0,
            argBoolean = argBoolean ?? false
        };
    }
}

public interface ISpawnBehavior
{
    public void SpawnObjectToLocal(in SpawnParamBase param,string runtimePath = null);
}