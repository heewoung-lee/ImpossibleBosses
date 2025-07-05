using GameManagers;
using GameManagers.Interface.UIManager;
using NetWork.NGO.UI;
using Unity.Netcode;
using UnityEngine;
using Util;
using Zenject;

namespace NetWork.NGO.Scene_NGO
{
    public class NgoStageTimerController : NetworkBehaviour
    {
        [Inject] private SceneManagerEx _sceneManagerEx;
        [Inject] private IUISceneManager _uiSceneManager;
        
        private Color _normalClockColor = "FF9300".HexCodetoConvertColor();
        private Color _allPlayerInPortalColor = "0084FF".HexCodetoConvertColor();


        private const float VillageStayTime = 300f;
        //private const float BossRoomStayTime = 60f;
        private const float BossRoomStayTime = 1f;
        private const float AllPlayerinPortalCount = 7f;
        private float _totalTime = 0;
        private float _currentTime = 0;

        private UIStageTimer _uiStageTimer;

        public UIStageTimer UIStageTimer
        {
            get
            {
                if (_uiStageTimer == null)
                {
                    _uiStageTimer = _uiSceneManager.GetOrCreateSceneUI<UIStageTimer>();
                }
                return _uiStageTimer;
            }
        }

        public float TotalTime
        {
            get
            {
                if (Mathf.Approximately(_totalTime, default))
                {
                    Define.Scene currentScene = _sceneManagerEx.CurrentScene;
                    _totalTime = currentScene == Define.Scene.GamePlayScene ? VillageStayTime : BossRoomStayTime;
                }
                return _totalTime;  
            }
        }


        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            SetHostTimer();
            if (IsHost == false) //클라이언트가 서버에 도는 시간을 가져와야 한다.
            {
                RequestTimeFromServerRpc();
            }
        }

        private void SetHostTimer()
        {
            if (IsHost == false)
                return;

            UIStageTimer.SetTimer(TotalTime);
        }


        [Rpc(SendTo.Server)]
        private void RequestTimeFromServerRpc(RpcParams rpcParams = default)
        {
            float currentCount = _uiSceneManager.Get_Scene_UI<UIStageTimer>().CurrentTime;
            ulong clientId = rpcParams.Receive.SenderClientId;

            SendTimeRpcToSpecificClientRpc(currentCount, RpcTarget.Single(clientId, RpcTargetUse.Temp));
        }

        [Rpc(SendTo.SpecifiedInParams)]
        private void SendTimeRpcToSpecificClientRpc(float currentCount, RpcParams rpcParams = default)
        {
            _currentTime = currentCount;
            UIStageTimer.SetTimer(TotalTime, _currentTime);
        }


        [Rpc(SendTo.ClientsAndHost)]
        public void SetPortalInAllPlayersCountRpc()
        {
            _currentTime = UIStageTimer.CurrentTime;
            UIStageTimer.SetTimer(AllPlayerinPortalCount, _allPlayerInPortalColor);
        }


        [Rpc(SendTo.ClientsAndHost)]
        public void SetNormalCountRpc()
        {
            UIStageTimer.SetTimer(TotalTime, _currentTime, _normalClockColor);
        }
    }
}
