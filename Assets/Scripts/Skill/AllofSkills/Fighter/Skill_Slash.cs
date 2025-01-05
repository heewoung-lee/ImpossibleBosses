using System.Collections;
using UnityEngine;

public class Skill_Slash : Skill_Immedialty
{
    public override Define.PlayerClass PlayerClass => Define.PlayerClass.Fighter;

    public override string SkillName => "강베기";

    public override float CoolTime => 5f;

    public override string EffectDescriptionText
    {
        get
        {
            if(_playerBaseStat == null)
            {
                _playerBaseStat = Managers.GameManagerEx.Player.GetComponent<BaseStats>();
            }
            return $"적에게{_playerBaseStat.Attack * Value}만큼 X3의 피해를 줍니다.";
        }
    }

    public override string ETCDescriptionText => "강하게 벤다";

    public override Sprite SkillconImage => Managers.ResourceManager.Load<Sprite>("Art/Player/SkillICon/WarriorSkill/SkillIcon/Slash");
    public override float Value => 1.5f;

    BaseController _playerBaseController;
    BaseStats _playerBaseStat;
    Module_Fighter_Class _fighter_Class;

    public override void InvokeSkill()
    {
        if (_playerBaseController == null || _fighter_Class == null)
        {
            _playerBaseController = Managers.GameManagerEx.Player.GetComponent<BaseController>();
            _fighter_Class = _playerBaseController.GetComponent<Module_Fighter_Class>();
        }
        _playerBaseController.CurrentStateType = _fighter_Class.SlashState;
        Managers.VFX_Manager.GenerateParticle("Player/SkillVFX/Figher_Slash",_playerBaseController.gameObject);
    }

    IEnumerator GenerateSlashEffect()
    {
        float duration = 0f;
        while (duration < 1f)
        {
            duration = _playerBaseController.Anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
            yield return null;
        }
        
       

    }
}