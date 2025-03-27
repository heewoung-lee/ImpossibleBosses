using System.Collections;
using UnityEngine;

public class Skill_Taunt : Skill_Immedialty
{

    private const float DURATION_PARTICLE = 5f;
    private BaseController _playerController;
    private Module_Fighter_Class _fighter_Class;
    private Collider[] _monsters;

    public override Define.PlayerClass PlayerClass => Define.PlayerClass.Fighter;
    public override string SkillName => "도발";
    public override float CoolTime => 10f;
    public override string EffectDescriptionText => $"적들에게 도발을해 나를 쫒아오도록한다";
    public override string ETCDescriptionText => "메롱";
    public override Sprite SkillconImage => Managers.ResourceManager.Load<Sprite>("Art/Player/SkillICon/WarriorSkill/SkillIcon/Taunt");
    public override float Value => 0f;
    public override BaseController PlayerController { 
        get => _playerController;
        protected set => _playerController = value;
    }
    public override Module_Player_Class Module_Player_Class {
        get => _fighter_Class;
        protected set => _fighter_Class = value as Module_Fighter_Class;
    }
    public override IState state => _fighter_Class.TauntState;
    public override void InvokeSkill()
    {
        base.InvokeSkill();
    }

    public override void AddInitailzeState()
    {
        base.AddInitailzeState();
        _fighter_Class.TauntState.UpdateStateEvent += PlaytheTaunt;
    }
    public void PlaytheTaunt()
    {
        foreach(Collider monster in _monsters)
        {
            if (monster.TryGetComponent(out BaseController controller))
            {
                controller.TargetObject = _playerController.gameObject;
            }
        }
    }

    public override void SkillAction()
    {
        LayerMask monsterLayerMask = LayerMask.GetMask("Monster");
        float skillRadius = float.MaxValue;

        Managers.VFX_Manager.GenerateParticle("Player/SkillVFX/Taunt_Player", _playerController.transform, DURATION_PARTICLE);

        _monsters = Physics.OverlapSphere(_playerController.transform.position, skillRadius, monsterLayerMask);
        foreach (Collider monster in _monsters)
        {
            HeadTr headTr = monster.GetComponentInChildren<HeadTr>();
            if (headTr != null)
            {
                Managers.VFX_Manager.GenerateParticle("Player/SkillVFX/Taunt_Enemy", headTr.transform, DURATION_PARTICLE);
            }
        }
    }
}