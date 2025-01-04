using BehaviorDesigner.Runtime.Tasks.Unity.UnityLayerMask;
using Google.Apis.Sheets.v4.Data;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class Skill_Buff_Roar : Skill_Duration
{
    public override Define.PlayerClass PlayerClass => Define.PlayerClass.Fighter;

    public override Sprite BuffIconImage => Managers.ResourceManager.Load<Sprite>("Art/UI/GUI Pro-FantasyRPG/ResourcesData/Sprites/Component/IconMisc/IconSet_Equip_Sword");

    public override float CoolTime => 5f;

    public override string DescriptionText => $"파티원들에게 10의 공격력을 부여합니다";

    public override Sprite SkillconImage => Managers.ResourceManager.Load<Sprite>("Art/Player/SkillICon/WarriorSkill/Roar");

    public override float Duration => 5f;

    public override string SkillName => "분노";

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

        foreach(Collider players_collider in _players)
        {
            Managers.VFX_Manager.GenerateParticle("States/Aura_acceleration", players_collider.transform.position,3f);
        }

    }

    public override void RemoveStats()
    {
        if (_players != null)
        {

        }
    }
}