using System;
using System.Linq;
using UnityEngine;

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
    }
}
