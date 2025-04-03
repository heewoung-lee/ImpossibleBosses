using System.Collections;
using UnityEngine;

public class Skill_Slash : Skill_Immedialty
{

    private BaseController _playerController;
    private PlayerStats _playerStat;
    private Module_Fighter_Class _fighter_Class;
    private AnimationClip _slashAnimClip;
    public AnimationClip SlashAnimClip
    {
        get
        {
            if (_slashAnimClip == null)
            {
                _slashAnimClip = _playerController.GetComponent<Module_Player_AnimInfo>().GetAnimationClip(_fighter_Class.Hash_Slash);
            }
            return _slashAnimClip;
        }
    }
    public float AttackDamage
    {
        get
        {
            if (_playerStat == null)
            {
                _playerStat = Managers.GameManagerEx.Player.GetComponent<PlayerStats>();
            }
            return _playerStat.Attack * Value;
        }
    }

    public PlayerStats PlayerStat
    {
        get
        {
            if(_playerStat == null)
            {
                _playerStat = Managers.GameManagerEx.Player.GetComponent<PlayerStats>();
            }
            return _playerStat;
        }
    }
    public override Define.PlayerClass PlayerClass => Define.PlayerClass.Fighter;
    public override string SkillName => "강베기";
    public override float CoolTime => 2f;
    public override string EffectDescriptionText => $"적에게{AttackDamage}만큼 X3의 피해를 줍니다.";
    public override string ETCDescriptionText => "강하게 벤다";
    public override Sprite SkillconImage => Managers.ResourceManager.Load<Sprite>("Art/Player/SkillICon/WarriorSkill/SkillIcon/Slash");
    public override float Value => 1.5f;
    public override BaseController PlayerController { 
        get => _playerController; 
        protected set => _playerController = value;
    }
    public override Module_Player_Class Module_Player_Class {
        get => _fighter_Class;
        protected set => _fighter_Class = value as Module_Fighter_Class;
    }

    public override IState state => _fighter_Class.SlashState;

    public override void InvokeSkill()
    {
      base.InvokeSkill();
    }
    public override void SkillAction()
    {
        Managers.VFX_Manager.GenerateParticle("Prefabs/Player/SkillVFX/Fighter_Slash", 
            _playerController.transform, 
            addParticleActionEvent: (slashParicle) =>
        {
            slashParicle.transform.rotation = _playerController.transform.rotation;
        });
        Managers.ManagersStartCoroutine(FrameInHit(PlayerStat, SlashAnimClip.length));
    }
    IEnumerator FrameInHit(PlayerStats stats, float animLength)
    {
        float duration = 0f;
        float[] hitFrames = new float[3] { 0.25f, 0.5f, 0.75f };
        int hitIndex = 0;
        while (duration < 1)
        {
            duration += Time.deltaTime / animLength;

            if (hitIndex < hitFrames.Length && duration > hitFrames[hitIndex])
            {
                TargetInSight.AttackTargetInSector(stats, (int)AttackDamage);
                hitIndex++;
            }
            yield return null;
        }
        duration = 0;
    }


}