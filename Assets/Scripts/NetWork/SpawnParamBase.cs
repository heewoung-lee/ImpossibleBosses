using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace NetWork
{
    public struct SpawnParamBase : INetworkSerializable
    {
        public float ArgFloat;
        public Vector3 ArgPosVector3;
        public FixedString512Bytes ArgString;
        public int ArgInteger;
        public ulong ArgUlong;
        public bool ArgBoolean;
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref ArgFloat);
            serializer.SerializeValue(ref ArgPosVector3);
            serializer.SerializeValue(ref ArgString);
            serializer.SerializeValue(ref ArgInteger);
            serializer.SerializeValue(ref ArgUlong);
            serializer.SerializeValue(ref ArgBoolean);
        }
        public static SpawnParamBase Create(float? argFloat = null, Vector3? argPosVector3 = null, string argString = null,
            int? argInteger = null,ulong? argUlong = null,bool? argBoolean = null)
        {
            return new SpawnParamBase
            {
                ArgPosVector3 = argPosVector3 ?? Vector3.zero,
                ArgString = argString == null ? default : new FixedString512Bytes(argString),
                ArgFloat = argFloat ?? 0f,
                ArgInteger = argInteger ?? 0,
                ArgUlong = argUlong ?? 0,
                ArgBoolean = argBoolean ?? false
            };
        }
    }
}