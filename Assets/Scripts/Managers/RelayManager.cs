using System;
using System.Threading.Tasks;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using UnityEngine;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.VisualScripting;
using Unity.Services.Multiplayer;

public class RelayManager
{

    private NetworkManager _netWorkManager;
    public async Task<string> StartHostWithRelay(int maxConnections)
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
            RelayServerData relaydata = AllocationUtils.ToRelayServerData(allocation, "dtls");
            if (Transform.FindAnyObjectByType<NetworkManager>() == false)
            {
                _netWorkManager = Managers.ResourceManager.InstantiatePrefab("Network/NetworkManager").GetComponent<NetworkManager>();
            }
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relaydata);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            if (NetworkManager.Singleton.StartHost())
            {
                return joinCode;
            }
            return null;

        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            return null;
        }
    }
    public async Task<bool> JoinGuestRelay(string joinCode)
    {
        try
        {
            JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            if (Transform.FindAnyObjectByType<NetworkManager>() == false)
            {
                _netWorkManager = Managers.ResourceManager.InstantiatePrefab("Network/NetworkManager").GetComponent<NetworkManager>();
            }
            RelayServerData relaydata = AllocationUtils.ToRelayServerData(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relaydata);
            return !string.IsNullOrEmpty(joinCode) && NetworkManager.Singleton.StartClient();
        }
        catch(Exception ex)
        {
            Debug.LogException(ex);
            return false;
        }
    }
}