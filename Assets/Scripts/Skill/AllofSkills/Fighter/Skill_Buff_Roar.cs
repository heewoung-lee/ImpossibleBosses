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

    public override string EffectDescriptionText => $"��Ƽ���鿡�� 10�� ���ݷ��� �ο��մϴ�";

    public override Sprite SkillconImage => Managers.ResourceManager.Load<Sprite>("Art/Player/SkillICon/WarriorSkill/SkillIcon/Roar");

    public override float Duration => 10f;//���ӽð�

    public override string SkillName => "�г�";

    public override string ETCDescriptionText => "ȭ������!";

    public override float Value => 10f;

    public override Buff_Modifier Buff_Modifier => _roarModifier;

    private Buffer_RoarModifier _roarModifier;

    Collider[] _players = null;
    BaseController _playerBaseController;
    Module_Fighter_Class _fighter_Class;

    public override void InvokeSkill()
    {
        if(_playerBaseController == null|| _fighter_Class == null)
        {
            _playerBaseController = Managers.GameManagerEx.Player.GetComponent<BaseController>();
            _fighter_Class = _playerBaseController.GetComponent<Module_Fighter_Class>();
        }
        _playerBaseController.CurrentStateType = _fighter_Class.RoarState;

        LayerMask playerLayerMask = LayerMask.GetMask("Player");
        float skillRadius = float.MaxValue;


        _players = Physics.OverlapSphere(_playerBaseController.transform.position, skillRadius, playerLayerMask);
        foreach (Collider players_collider in _players)
        {
            GameObject roarParticle = Managers.VFX_Manager.GenerateParticle("States/Aura_Roar", players_collider.gameObject, Duration);

            if(players_collider.TryGetComponent(out BaseStats playerStats))
            {
               Managers.BufferManager.InitBuff(playerStats, Duration, _roarModifier, Value);
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