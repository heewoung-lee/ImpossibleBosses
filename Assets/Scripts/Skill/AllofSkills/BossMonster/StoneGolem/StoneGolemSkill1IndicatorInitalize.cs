using GameManagers;
using GameManagers.Interface.Resources_Interface;
using GameManagers.Interface.ResourcesManager;
using NetWork;
using NetWork.NGO.Interface;
using Stats.BaseStats;
using UnityEngine;
using Util;
using Zenject;

namespace Skill.AllofSkills.BossMonster.StoneGolem
{
    public class StoneGolemSkill1IndicatorInitalize : Poolable, ISpawnBehavior
    {
        private const float Skill1Radius = 2f;
        private const float Skill1Arc = 360f;
        [Inject] private IInstantiate _instantiate;
        public void SpawnObjectToLocal(in SpawnParamBase spawnparam, string runtimePath = null)
        {
            IIndicatorBahaviour projector = _instantiate.InstantiateByPath(runtimePath,Managers.VFXManager.VFXRoot).GetComponent<IIndicatorBahaviour>();
            IAttackRange attacker = (projector as Component).GetComponent<IAttackRange>();
            int attackDamage = spawnparam.ArgInteger;
            float durationTime = spawnparam.ArgFloat;
            projector.SetValue(Skill1Radius, Skill1Arc, spawnparam.ArgPosVector3, durationTime, Attack);
            void Attack()
            {
                TargetInSight.AttackTargetInCircle(attacker, projector.Radius, attackDamage);
            }
        }
    }
}
