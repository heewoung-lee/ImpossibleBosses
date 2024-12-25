using BaseStates;
using System.Collections.Generic;
using UnityEngine;

public enum GolemAttackType
{
    NormalAttack,
    Skill1,
    Skill2,
}
public class BossGolemController : BossController<GolemAttackType>
{
  
    private const float ATTACK_PRE_FRAME = 0.35f;
    private const float SKILL1_PRE_FRAME = 0.8f;
    private const float SKILL2_PRE_FRAME = 0.3f;

    public override GolemAttackType AttackType { get; set; }

    private Dictionary<GolemAttackType, float> _attackTypeDict = new Dictionary<GolemAttackType, float>();
    public override Dictionary<GolemAttackType,float> AttackTypeDict => _attackTypeDict;

    public float Attack_Pre_Frame { get => ATTACK_PRE_FRAME; }
    public float Skill2_Pre_Frame { get => SKILL2_PRE_FRAME; }
    public float Skill1_Pre_Frame { get => SKILL1_PRE_FRAME; }


    private int[] _golem_Attacks = new int[2]
    {
        Animator.StringToHash("Golem_Attack1"),
        Animator.StringToHash("Golem_Attack2")
    };
    private int _golem_Skill2 = Enemy_Anim_Hash.Golem_Skill;
    private int _golem_Skill1 = Enemy_Anim_Hash.Golem_Attacked;

    protected override int Hash_Idle => Enemy_Anim_Hash.Golem_Idle;
    protected override int Hash_Move => Enemy_Anim_Hash.Golem_Walk;
    protected override int Hash_Attack => GetAnimHash(AttackType);
    protected override int Hash_Die => Enemy_Anim_Hash.Golem_Dead;

    public override AttackState Base_Attackstate => throw new System.NotImplementedException();
    public override IDleState Base_IDleState => throw new System.NotImplementedException();
    public override DieState Base_DieState => throw new System.NotImplementedException();
    public override MoveState Base_MoveState => throw new System.NotImplementedException();

    private int GetAnimHash(GolemAttackType attackType)
    {
        switch (attackType)
        {
            case GolemAttackType.NormalAttack:
                return _golem_Attacks[UnityEngine.Random.Range(0, 2)];
            case GolemAttackType.Skill1:
                return _golem_Skill1;
            case GolemAttackType.Skill2:
                return _golem_Skill2;
        }
        return _golem_Attacks[UnityEngine.Random.Range(0, 2)];
    }

    protected override void AwakeInit()
    {
        _attackTypeDict.Add(GolemAttackType.NormalAttack, ATTACK_PRE_FRAME);
        _attackTypeDict.Add(GolemAttackType.Skill1, SKILL1_PRE_FRAME);
        _attackTypeDict.Add(GolemAttackType.Skill2, SKILL2_PRE_FRAME);
    }

    protected override void StartInit()
    {

    }

}