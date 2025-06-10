using UnityEngine;

namespace Data
{
    public class PlayerAnimHash
    {

        public static int Idle { get; private set; } = Animator.StringToHash("Idle");
        public static int Run { get; private set; } = Animator.StringToHash("Run");
        public static int Die { get; private set; } = Animator.StringToHash("Die");
        public static int Attack { get; private set; } = Animator.StringToHash("Attack");
    }
    public class EnemyAnimHash
    {
        public static int GolemWait { get; private set; } = Animator.StringToHash("Golem_Wait");
        public static int GolemIdle { get; private set; } = Animator.StringToHash("Golem_Idle");
        public static int GolemRise { get; private set; } = Animator.StringToHash("Golem_Rise");
        public static int GolemWalk { get; private set; } = Animator.StringToHash("Golem_Walk");
        public static int GolemAttack1 { get; private set; } = Animator.StringToHash("Golem_Attack1");
        public static int GolemAttack2 { get; private set; } = Animator.StringToHash("Golem_Attack2");
        public static int GolemSkill { get; private set; } = Animator.StringToHash("Golem_Skill");
        public static int GolemAttacked { get; private set; } = Animator.StringToHash("Golem_Attacked");
        public static int GolemDead { get; private set; } = Animator.StringToHash("Golem_Dead");

    }
}