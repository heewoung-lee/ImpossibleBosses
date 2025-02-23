using System;
using Unity.Netcode;
using UnityEngine;

public class NetWorkManager : NetworkBehaviour
{
    // ��Ʈ��ũ �̺�Ʈ�� ���� ��������Ʈ�� �̺�Ʈ ����
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
            // ���� ����
            NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnected;
        }
    }
}
