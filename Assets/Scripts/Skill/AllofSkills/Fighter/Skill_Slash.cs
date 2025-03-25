using System.Collections;
using UnityEngine;

public class Skill_Slash : Skill_Immedialty
{
    public override Define.PlayerClass PlayerClass => Define.PlayerClass.Fighter;

    public override string SkillName => "강베기";

    public override float CoolTime => 2f;

    public override string EffectDescriptionText
    {
        get
        {
            return $"적에게{AttackDamage}만큼 X3의 피해를 줍니다.";
        }
    }

    public override string ETCDescriptionText => "강하게 벤다";

    public override Sprite SkillconImage => Managers.ResourceManager.Load<Sprite>("Art/Player/SkillICon/WarriorSkill/SkillIcon/Slash");
    public override float Value => 1.5f;

    BaseController _playerController;
    PlayerStats _playerStat;
    Module_Fighter_Class _fighter_Class;

    AnimationClip _slashAnimClip;

    AnimationClip SlashAnimClip
    {
        get
        {
            if(_slashAnimClip == null)
            {
                _slashAnimClip = _playerController.GetComponent<Module_Player_AnimInfo>().GetAnimationClip(_fighter_Class.Hash_Slash);
            }
            return _slashAnimClip;
        }
    }
    float AttackDamage
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

    public override void InvokeSkill()
    {
        if (_playerController == null || _fighter_Class == null)
        {
            _playerController = Managers.GameManagerEx.Player.GetComponent<BaseController>();
            _fighter_Class = _playerController.GetComponent<Module_Fighter_Class>();
            _playerStat = _playerController.GetComponent<PlayerStats>();
            _playerController.StateAnimDict.RegisterState(_fighter_Class.SlashState, PlaytheSlah);
        }
        _playerController.CurrentStateType = _fighter_Class.SlashState;
    }
    public void PlaytheSlah()
    {
        Managers.VFX_Manager.GenerateParticle("Player/SkillVFX/Fighter_Slash", _playerController.transform, addParticleActionEvent: (slashParicle) =>
        {
            slashParicle.transform.rotation = _playerController.transform.rotation;
        });
        Managers.ManagersStartCoroutine(FrameInHit(_playerStat, SlashAnimClip.length));
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