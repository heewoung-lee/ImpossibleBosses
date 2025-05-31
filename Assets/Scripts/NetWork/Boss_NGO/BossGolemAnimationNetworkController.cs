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
}
