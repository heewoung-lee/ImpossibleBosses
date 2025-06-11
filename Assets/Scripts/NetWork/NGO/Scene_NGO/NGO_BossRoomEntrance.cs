using System;
using GameManagers;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class NGO_BossRoomEntrance : NetworkBehaviourBase
{
    private Vector3 _town_Portal_Position = new Vector3(15f, 0.15f, 32);
    public Vector3 Town_Portal_Position => _town_Portal_Position;


    private NGO_Stage_Timer_Controller _timer_controller;


    public NGO_Stage_Timer_Controller Timer_controller
    {
        get
        {
            if(_timer_controller == null)
            {
                
                foreach(NetworkObject ngo in Managers.RelayManager.NetworkManagerEx.SpawnManager.SpawnedObjectsList)
                {
                    if(ngo.TryGetComponent(out NGO_Stage_Timer_Controller stage_Timer_controller))
                    {
                        _timer_controller = stage_Timer_controller;
                        break;
                    }
                }
            }
            return _timer_controller;
        }
    }
    private Collider _portalTrigger;

    NetworkVariable<int> _playerCountInPortal = new NetworkVariable<int>
        (0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);


    NetworkVariable<bool> _isAllplayersinPortal = new NetworkVariable<bool>
        (false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

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

       if(newValue == Managers.RelayManager.NetworkManagerEx.ConnectedClientsList.Count)
        {
            _isAllplayersinPortal.Value = true;
            Timer_controller.SetPortalInAllPlayersCountRpc();
        }
        else
        {
            if (_isAllplayersinPortal.Value == false)
                return;

            _isAllplayersinPortal.Value = false;
            Timer_controller.SetNormalCountRpc();
        }
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
