using GameManagers;
using NetWork;
using NetWork.NGO.Interface;
using UnityEngine;

namespace Skill.AllofSkills.BossMonster.StoneGolem
{
    public class StoneGolemSkill1IndicatorInitalize : Poolable, ISpawnBehavior
    {
        private const float Skill1Radius = 2f;
        private const float Skill1Arc = 360f;

        public void SpawnObjectToLocal(in SpawnParamBase spawnparam, string runtimePath = null)
        {
            IIndicatorBahaviour projector = Managers.ResourceManager.Instantiate(runtimePath,Managers.VFXManager.VFXRoot).GetComponent<IIndicatorBahaviour>();
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
