using Unity.Netcode;
using UnityEngine;

public class BossGolemAnimationNetworkController : NetworkBehaviour
{
    BossGolemController _bossGolemController;
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        _bossGolemController = GetComponent<BossGolemController>();
    }
    [Rpc(SendTo.ClientsAndHost)]
    public void SetBossSkill1Rpc()
    {
        _bossGolemController.CurrentStateType = _bossGolemController.BossSkill1State;
    }
    [Rpc(SendTo.ClientsAndHost)]
    public void SetBossAttackRpc()
    {
        _bossGolemController.CurrentStateType = _bossGolemController.Base_Attackstate;
    }
    [Rpc(SendTo.ClientsAndHost)]
    public void SetBossSkill2Rpc()
    {
        _bossGolemController.CurrentStateType = _bossGolemController.BossSkill2State;
    }


}
