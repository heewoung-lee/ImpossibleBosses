using System;
using Unity.Netcode;
using UnityEngine;

public interface ISpawnBehavior<T> where T : struct, INetworkSerializable
{
    void Spawn(in T param);
}