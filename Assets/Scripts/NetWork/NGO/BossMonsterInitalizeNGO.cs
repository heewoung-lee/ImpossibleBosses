using BehaviorDesigner.Runtime;

public class BossMonsterInitalizeNGO : NetworkBehaviourBase
{
    protected override void AwakeInit()
    {
    }

    protected override void StartInit()
    {
    }


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Managers.GameManagerEx.SetBossMonster(gameObject);

        if (IsHost == false)
        {
            GetComponent<BossController>().enabled = false;
            GetComponent<BossGolemStats>().enabled = false;
            GetComponent<BehaviorTree>().enabled = false;
        }
    }
}
