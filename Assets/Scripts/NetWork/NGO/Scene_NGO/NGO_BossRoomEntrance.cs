using System;
using Unity.Netcode;
using UnityEngine;

public class NGO_BossRoomEntrance : NetworkBehaviourBase
{
    private Vector3 _town_Portal_Position = new Vector3(15f, 0.15f, 32);
    public Vector3 Town_Portal_Position => _town_Portal_Position;

    private Collider _portalTrigger;


    NetworkVariable<int> _playerCountInPortal = new NetworkVariable<int>
        (0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsHost == false)
            return;

        gameObject.transform.position = _town_Portal_Position;


        _playerCountInPortal.OnValueChanged -= OnChangedCountPlayer;
        _playerCountInPortal.OnValueChanged += OnChangedCountPlayer;
    }



    private void OnChangedCountPlayer(int previousValue, int newValue)
    {
        //플레이어가 참여하면, UI업데이트

        //모든 플레이어가 다들어왔으면 코루틴 돌리기
    }

    protected override void StartInit()
    {
        _portalTrigger = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsHost == false)
            return;

        if (other.GetComponent<PlayerStats>() != null)
        {
            _playerCountInPortal.Value++;

            if (other.TryGetComponent(out NetworkObject playerngo))
            {
                EnteredPlayerInPortalRpc(playerngo.NetworkObjectId);
            }
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (IsHost == false)
            return;

        if (other.GetComponent<PlayerStats>() != null)
        {
            _playerCountInPortal.Value--;
            if (other.TryGetComponent(out NetworkObject playerngo))
            {
                ExitedPlayerInPortalRpc(playerngo.NetworkObjectId);
            }
        }
    }

    protected override void AwakeInit()
    {
    }



    [Rpc(SendTo.ClientsAndHost)]
    public void EnteredPlayerInPortalRpc(ulong playerIndex)
    {
        if (Managers.RelayManager.NetworkManagerEx.SpawnManager.SpawnedObjects.TryGetValue(playerIndex,out NetworkObject player))
        {
            player.gameObject.TryGetComponentInChildren(out UI_PortalIndicator indicator);
            indicator.SetIndicatorOn();
        }
    }


    [Rpc(SendTo.ClientsAndHost)]
    public void ExitedPlayerInPortalRpc(ulong playerIndex)
    {
        if (Managers.RelayManager.NetworkManagerEx.SpawnManager.SpawnedObjects.TryGetValue(playerIndex, out NetworkObject player))
        {
            player.gameObject.TryGetComponentInChildren(out UI_PortalIndicator indicator);
            indicator.SetIndicatorOff();
        }
    }
}
