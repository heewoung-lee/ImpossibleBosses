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

    BaseController _playerBaseController;
    PlayerStats _playerStat;
    Module_Fighter_Class _fighter_Class;
    float AttackDamage
    {
        get
        {
            if(_playerStat == null)
            {
                _playerStat = Managers.GameManagerEx.Player.GetComponent<PlayerStats>();
            }
            return _playerStat.Attack * Value;
        }
    }

    public override void InvokeSkill()
    {
        if (_playerBaseController == null || _fighter_Class == null)
        {
            _playerBaseController = Managers.GameManagerEx.Player.GetComponent<BaseController>();
            _fighter_Class = _playerBaseController.GetComponent<Module_Fighter_Class>();
            _playerStat = _playerBaseController.GetComponent<PlayerStats>();
        }
        _playerBaseController.CurrentStateType = _fighter_Class.SlashState;
        GameObject slashParticle = Managers.VFX_Manager.GenerateParticle("Player/SkillVFX/Fighter_Slash",_playerBaseController.gameObject);
        slashParticle.transform.rotation = _playerBaseController.transform.rotation;
        Managers.ManagersStartCoroutine(FrameInHit(_playerStat));
    }

    IEnumerator FrameInHit(PlayerStats stats)
    {
        float duration = 0f;
        float firstHitFrame = 0.25f;
        float secondHitFrame = 0.75f;
        float thirdHitFrame = 1f;
        bool isfirstHit = false;
        bool issecondHit = false;
        bool isthirdHit = false;

        float animationLength = _playerBaseController.Anim.GetCurrentAnimatorStateInfo(0).length;
        while (duration < 1)
        {
            duration += Time.deltaTime / animationLength;
            Debug.Log(duration);
            if(duration> firstHitFrame && isfirstHit == false)
            {
                Debug.Log("첫번째 공격");
                TargetInSight.AttackTargetInSector(stats, (int)AttackDamage);
                isfirstHit = true;
            }
            else if(isfirstHit == true && duration > secondHitFrame && issecondHit == false)
            {
                Debug.Log($"두번째 공격{duration}");
                TargetInSight.AttackTargetInSector(stats, (int)AttackDamage);
                issecondHit = true;
            }
            else if(isfirstHit == true && issecondHit == true && duration > thirdHitFrame && isthirdHit == false)
            {
                Debug.Log($"세번째 공격{duration}");
                TargetInSight.AttackTargetInSector(stats, (int)AttackDamage);
                isthirdHit = true;
            }
            yield return null;
        }
        duration = 0;
    }
}