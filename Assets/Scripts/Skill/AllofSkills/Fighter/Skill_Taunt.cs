using System.Collections;
using UnityEngine;

public class Skill_Taunt : Skill_Immedialty
{

    private const float DURATION_PARTICLE = 5f;
    public override Define.PlayerClass PlayerClass => Define.PlayerClass.Fighter;

    public override string SkillName => "도발";

    public override float CoolTime => 10f;

    public override string EffectDescriptionText
    {
        get
        {
            return $"적들에게 도발을해 나를 쫒아오도록한다";
        }
    }

    public override string ETCDescriptionText => "메롱";

    public override Sprite SkillconImage => Managers.ResourceManager.Load<Sprite>("Art/Player/SkillICon/WarriorSkill/SkillIcon/Taunt");
    public override float Value => 0f;

    BaseController _playerController;
    Module_Fighter_Class _fighter_Class;
    Collider[] _monsters;

    public override void InvokeSkill()
    {
        if (_playerController == null || _fighter_Class == null)
        {
            _playerController = Managers.GameManagerEx.Player.GetComponent<BaseController>();
            _fighter_Class = _playerController.GetComponent<Module_Fighter_Class>();
            _playerController.StateAnimDict.RegisterState(_fighter_Class.TauntState, GenerateParticle);
            _fighter_Class.TauntState.UpdateStateEvent += PlaytheTaunt;
        }
        _playerController.CurrentStateType = _fighter_Class.TauntState;

    }

    public void GenerateParticle()
    {
        LayerMask monsterLayerMask = LayerMask.GetMask("Monster");
        float skillRadius = float.MaxValue;

        Managers.VFX_Manager.GenerateLocalParticle("Player/SkillVFX/Taunt_Player", _playerController.transform, DURATION_PARTICLE);

        _monsters = Physics.OverlapSphere(_playerController.transform.position, skillRadius, monsterLayerMask);
        foreach (Collider monster in _monsters)
        {
            HeadTr headTr  = monster.GetComponentInChildren<HeadTr>();
            if (headTr != null)
            {
                GameObject tauntParticle = Managers.VFX_Manager.GenerateLocalParticle("Player/SkillVFX/Taunt_Enemy", headTr.transform, DURATION_PARTICLE);
            }
        }
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
}