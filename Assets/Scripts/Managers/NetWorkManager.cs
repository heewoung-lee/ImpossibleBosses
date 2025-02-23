using System;
using Unity.Netcode;
using UnityEngine;

public class NetWorkManager : NetworkBehaviour
{
    // 네트워크 이벤트를 위한 델리게이트와 이벤트 선언
    public event Action<ulong> OnPlayerConnected;
    public event Action<ulong> OnPlayerDisconnected;

    public override void OnNetworkSpawn()
    {
        if (IsHost)
        {
           Managers.RelayManager.NetWorkManager.OnClientConnectedCallback += HandleClientConnected;
           Managers.RelayManager.NetWorkManager.OnClientDisconnectCallback += HandleClientDisconnected;
        }
    }

    private void HandleClientConnected(ulong clientId)
    {
        Debug.Log($"Player connected: {clientId}");
        OnPlayerConnected?.Invoke(clientId);
    }

    private void HandleClientDisconnected(ulong clientId)
    {
        Debug.Log($"Player disconnected: {clientId}");
        OnPlayerDisconnected?.Invoke(clientId);
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            // 구독 해제
            NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnected;
        }
    }
}
