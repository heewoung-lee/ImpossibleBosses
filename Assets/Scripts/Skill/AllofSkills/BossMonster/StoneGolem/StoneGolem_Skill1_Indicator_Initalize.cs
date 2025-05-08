using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class StoneGolem_Skill1_Indicator_Initalize : Poolable, ISpawnBehavior
{
    private const float SKILL1_RADIUS = 2f;
    private const float SKILL1_ARC = 360f;
    private const float AttackDelayTime = 1f;

    public void SpawnObjectToLocal(in SpawnParamBase spawnparam, string runtimePath = null)
    {
        IIndicatorBahaviour projector = Managers.ResourceManager.Instantiate(runtimePath,Managers.VFX_Manager.VFX_Root).GetComponent<IIndicatorBahaviour>();
        IAttackRange attacker = (projector as Component).GetComponent<IAttackRange>();
        int attackDamage = spawnparam.argInteger;
        projector.SetValue(SKILL1_RADIUS, SKILL1_ARC, spawnparam.argPosVector3, Attack);
        void Attack()
        {
            TargetInSight.AttackTargetInCircle(attacker, projector.Radius, attackDamage);
        }
    }
}
