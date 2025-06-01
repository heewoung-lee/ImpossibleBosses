using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Unity.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class BossGolemAnimationNetworkController : NetworkBehaviour
{
    BossGolemController _bossGolemController;
    private Dictionary<string,IState> _bossAttackStateDict = new Dictionary<string, IState>();
    public Dictionary<string, IState> BossAttackStateDict => _bossAttackStateDict;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        _bossGolemController = GetComponent<BossGolemController>();

        foreach(IState istate in _bossGolemController.AttackStopTimingRatioDict.Keys)
        {
            string istateName = istate.GetType().Name;
            _bossAttackStateDict.Add(istateName, istate);
        }
    }
    [Rpc(SendTo.ClientsAndHost)]
    public void SetBossStateRpc(FixedString512Bytes stateName)
    {
        _bossGolemController.CurrentStateType = _bossAttackStateDict[stateName.ToString()];
    }

    public void SyncBossStateToClients<T>(T state) where T : IState
    {
            if (Managers.RelayManager.NetworkManagerEx.IsHost == false)
                return;

            string name = typeof(T).Name;

        SetBossStateRpc(name);

    }
}
