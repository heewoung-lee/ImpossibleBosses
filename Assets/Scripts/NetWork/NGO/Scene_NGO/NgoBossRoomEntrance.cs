using GameManagers;
using NetWork.BaseNGO;
using Stats;
using UI.WorldSpace;
using Unity.Netcode;
using UnityEngine;
using Util;

namespace NetWork.NGO.Scene_NGO
{
    public class NgoBossRoomEntrance : NetworkBehaviourBase
    {
        private Vector3 _townPortalPosition = new Vector3(15f, 0.15f, 32);
        private NgoStageTimerController _timerController;
        public NgoStageTimerController TimerController
        {
            get
            {
                if(_timerController == null)
                {
                
                    foreach(NetworkObject ngo in Managers.RelayManager.NetworkManagerEx.SpawnManager.SpawnedObjectsList)
                    {
                        if(ngo.TryGetComponent(out NgoStageTimerController stageTimerController))
                        {
                            _timerController = stageTimerController;
                            break;
                        }
                    }
                }
                return _timerController;
            }
        }

        NetworkVariable<int> _playerCountInPortal = new NetworkVariable<int>
            (0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);


        NetworkVariable<bool> _isAllplayersinPortal = new NetworkVariable<bool>
            (false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (IsHost == false)
                return;

            gameObject.transform.position = _townPortalPosition;


            _playerCountInPortal.OnValueChanged -= OnChangedCountPlayer;
            _playerCountInPortal.OnValueChanged += OnChangedCountPlayer;
        }



        private void OnChangedCountPlayer(int previousValue, int newValue)
        {

            if(newValue == Managers.RelayManager.NetworkManagerEx.ConnectedClientsList.Count)
            {
                _isAllplayersinPortal.Value = true;
                TimerController.SetPortalInAllPlayersCountRpc();
            }
            else
            {
                if (_isAllplayersinPortal.Value == false)
                    return;

                _isAllplayersinPortal.Value = false;
                TimerController.SetNormalCountRpc();
            }
        }

        protected override void StartInit()
        {
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
                player.gameObject.TryGetComponentInChildren(out UIPortalIndicator indicator);
                indicator.SetIndicatorOn();
            }
        }


        [Rpc(SendTo.ClientsAndHost)]
        public void ExitedPlayerInPortalRpc(ulong playerIndex)
        {
            if (Managers.RelayManager.NetworkManagerEx.SpawnManager.SpawnedObjects.TryGetValue(playerIndex, out NetworkObject player))
            {
                player.gameObject.TryGetComponentInChildren(out UIPortalIndicator indicator);
                indicator.SetIndicatorOff();
            }
        }
    }
}
