using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using static UnityEditor.PlayerSettings;


public struct DustParicleParams : INetworkSerializable
{
    public Vector3 pos;
    public FixedString512Bytes path;
    public float durationTime;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref pos);
        serializer.SerializeValue(ref path);
        serializer.SerializeValue(ref durationTime);
    }
}


public class DustParticleInitalize : Poolable,ISpawnBehavior<DustParicleParams>
{
    public void Spawn(in DustParicleParams param)
    {
        Managers.VFX_Manager.GenerateParticle(param.path.ToString(), param.pos, param.durationTime);
    }
}
