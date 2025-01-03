using BehaviorDesigner.Runtime.Tasks.Unity.UnityLayerMask;
using Google.Apis.Sheets.v4.Data;
using Unity.VisualScripting;
using UnityEngine;

public class Skill_Buff_Roar : Skill_Duration_Buff
{
    public override Define.PlayerClass PlayerClass => Define.PlayerClass.Fighter;

    public override Sprite BuffIconImage => Managers.ResourceManager.Load<Sprite>("Art/UI/GUI Pro-FantasyRPG/ResourcesData/Sprites/Component/IconMisc/IconSet_Equip_Sword");

    public override StatType StatType => StatType.Attack;

    public override float CoolTime => 5f;

    public override string DescriptionText => $"파티원들에게 10의 공격력을 부여합니다";

    protected override string skillName => "분노";

    public override Sprite SkillconImage => Managers.ResourceManager.Load<Sprite>("Art/Player/SkillICon/WarriorSkill/Roar");

    Collider[] _players = null;

    BaseController _playerBaseController;
    Module_Fighter_Class _fighter_Class;
    public override void ApplyStats(BaseStats stats, float value)
    {
        LayerMask playerLayerMask = LayerMask.GetMask("Player");
        float skillRadius = float.MaxValue;


        _players = Physics.OverlapSphere(stats.transform.position, skillRadius, playerLayerMask);


        
        //2. 모든 플레이어들을 감지
        //3. 버프 넣어주고
        //4. 쿨타임 돌리고
        //5. 버프 시간이 끝나면 RemoveStats실행

    }

    public override void RemoveStats(BaseStats stats, float value)
    {
        if(_players != null)
        {

        }
    }

    public override void InvokeSkill()
    {
        if(_playerBaseController == null|| _fighter_Class == null)
        {
            _playerBaseController = Managers.GameManagerEx.Player.GetComponent<BaseController>();
            _fighter_Class = _playerBaseController.GetComponent<Module_Fighter_Class>();
        }
        _playerBaseController.CurrentStateType = _fighter_Class.RoarState;
    }
}