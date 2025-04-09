using System;
using Unity.Netcode;
using UnityEngine;

public class NGO_BossRoomEntrance : NetworkBehaviourBase
{
    private Vector3 _town_Portal_Position = new Vector3(15f, 0.15f, 32);
    public Vector3 Town_Portal_Position => _town_Portal_Position;

    private Collider _portalTrigger;

    private NGO_UI_Stage_Timer _timer;

    public NGO_UI_Stage_Timer Timer
    {
        get
        {
            if (_timer == null)
            {
                _timer = Managers.UI_Manager.Get_Scene_UI<NGO_UI_Stage_Timer>();
            }
            return _timer;
        }
    }


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
       if(newValue == Managers.RelayManager.NetworkManagerEx.ConnectedClientsList.Count)
        {
            Timer.SetIscheckPlayerInPortal(true);
        }
        else
        {
            Timer.SetIscheckPlayerInPortal(false);
        }
        //newValue가 현재 있는 플레이어 수와 같은지 확인.
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
