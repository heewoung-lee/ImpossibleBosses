using System.Collections.Generic;
using Controller.BossState;
using GameManagers;
using Unity.Collections;
using Unity.Netcode;
using Zenject;
using IState = Controller.ControllerStats.IState;

namespace NetWork.Boss_NGO
{
    public class BossGolemAnimationNetworkController : NetworkBehaviour
    {
        [Inject] private RelayManager _relayManager;

        BossGolemController _bossGolemController;
        private Dictionary<string,IState> _bossAttackStateDict = new Dictionary<string, IState>();
        public Dictionary<string, IState> BossAttackStateDict => _bossAttackStateDict;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            _bossGolemController = GetComponent<BossGolemController>();

            foreach(IState istate in _bossGolemController.StateAnimDict.StateDict.Keys)
            {
                string istateName = istate.GetType().Name;
                _bossAttackStateDict.Add(istateName, istate);
            }
        }
        [Rpc(SendTo.ClientsAndHost)]
        private void SetBossStateRpc(FixedString512Bytes stateName)
        {
            _bossGolemController.CurrentStateType = _bossAttackStateDict[stateName.ToString()];
        }

        public void SyncBossStateToClients<T>(T state) where T : IState
        {
            if (_relayManager.NetworkManagerEx.IsHost == false)
                return;

            string typename = state.GetType().Name;

            SetBossStateRpc(typename);

        }
    }
}
