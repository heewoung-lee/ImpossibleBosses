using BehaviorDesigner.Runtime.Tasks.Unity.UnityLayerMask;
using Google.Apis.Sheets.v4.Data;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class Skill_Buff_Roar : Skill_Duration
{
    public Skill_Buff_Roar()
    {
        _buffIconImage =  Managers.ResourceManager.Load<Sprite>("Art/Player/SkillICon/WarriorSkill/BuffSkillIcon/Icon_Booster_Power");
        _roarModifier = new Buffer_RoarModifier(_buffIconImage);
    }
    public override Define.PlayerClass PlayerClass => Define.PlayerClass.Fighter;

    public override Sprite BuffIconImage => _buffIconImage;

    private Sprite _buffIconImage;
    public override float CoolTime => 5f;

    public override string EffectDescriptionText => $"파티원들에게 10의 공격력을 부여합니다";

    public override Sprite SkillconImage => Managers.ResourceManager.Load<Sprite>("Art/Player/SkillICon/WarriorSkill/SkillIcon/Roar");

    public override float SkillDuration => 10f;//지속시간

    public override string SkillName => "분노";

    public override string ETCDescriptionText => "화가난다!";

    public override float Value => 10f;

    public override Buff_Modifier Buff_Modifier => _roarModifier;

    private Buffer_RoarModifier _roarModifier;

    Collider[] _players = null;
    BaseController _playerController;
    Module_Fighter_Class _fighter_Class;

    public override void InvokeSkill()
    {
        if(_playerController == null|| _fighter_Class == null)
        {
            _playerController = Managers.GameManagerEx.Player.GetComponent<BaseController>();
            _fighter_Class = _playerController.GetComponent<Module_Fighter_Class>();
            _playerController.StateAnimDict.RegisterState(_fighter_Class.RoarState, PlaytheRoar);
        }
        _playerController.CurrentStateType = _fighter_Class.RoarState;
    }

    public void PlaytheRoar()
    {
        LayerMask playerLayerMask = LayerMask.GetMask("Player");
        float skillRadius = float.MaxValue;
        _players = Physics.OverlapSphere(_playerController.transform.position, skillRadius, playerLayerMask);
        foreach (Collider players_collider in _players)
        {
            if (players_collider.TryGetComponent(out BaseStats playerStats))
            {
                GameObject roarParticle = Managers.VFX_Manager.GenerateLocalParticle("Player/SkillVFX/Aura_Roar", playerStats.transform, SkillDuration);
                Managers.BufferManager.InitBuff(playerStats, SkillDuration, _roarModifier, Value);
            }
        }
    }

    public override void RemoveStats()
    {
        if (_players != null)
        {

        }
    }

}