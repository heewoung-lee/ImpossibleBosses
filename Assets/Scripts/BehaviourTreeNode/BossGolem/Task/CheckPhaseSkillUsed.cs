using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace BehaviourTreeNode.BossGolem.Task
{
    public class CheckPhaseSkillUsed : Conditional
    {

        public readonly SharedBool IsPhaseSkillUsed;

        public override TaskStatus OnUpdate()
        {
            if(IsPhaseSkillUsed.Value == false)
            {
                IsPhaseSkillUsed.Value = true;
                return TaskStatus.Success;
            }
            return TaskStatus.Failure;
        }
        public override void OnEnd()
        {
            base.OnEnd();
        }
    }
}
