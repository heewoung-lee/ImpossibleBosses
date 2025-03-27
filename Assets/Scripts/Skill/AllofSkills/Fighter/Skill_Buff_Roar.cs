using UnityEngine;

public class Skill_Buff_Roar : Skill_Duration
{
    public Skill_Buff_Roar()
    {
        _buffIconImage = Managers.ResourceManager.Load<Sprite>(Buff_IconImage_Path);
        _roarModifier = new Buffer_RoarModifier(_buffIconImage);
    }

    private Buffer_RoarModifier _roarModifier;
    private Collider[] _players = null;
    private BaseController _playerController;
    private Module_Fighter_Class _fighter_Class;
    private Sprite _buffIconImage;

    public override string Buff_IconImage_Path => "Art/Player/SkillICon/WarriorSkill/BuffSkillIcon/Icon_Booster_Power";
    public override Define.PlayerClass PlayerClass => Define.PlayerClass.Fighter;
    public override Sprite BuffIconImage => _buffIconImage;
    public override float CoolTime => 5f;
    public override string EffectDescriptionText => $"파티원들에게 10의 공격력을 부여합니다";
    public override Sprite SkillconImage => Managers.ResourceManager.Load<Sprite>("Art/Player/SkillICon/WarriorSkill/SkillIcon/Roar");
    public override float SkillDuration => 10f;//지속시간
    public override string SkillName => "분노";
    public override string ETCDescriptionText => "화가난다!";
    public override float Value => 10f;
    public override Buff_Modifier Buff_Modifier => _roarModifier;
    public override IState state => _fighter_Class.RoarState;
    public override BaseController PlayerController
    {
        get => _playerController;
        protected set => _playerController = value;
    }
    public override Module_Player_Class Module_Player_Class
    {
        get => _fighter_Class;
        protected set => _fighter_Class = value as Module_Fighter_Class;
    }


    public override void InvokeSkill()
    {
        base.InvokeSkill();
    }

    public override void RemoveStats()
    {
        if (_players != null)
        {

        }
    }

    public override void SkillAction()
    {
        _players = Managers.BufferManager.DetectedPlayers();

        Managers.BufferManager.ALL_Character_ApplyBuffAndCreateParticle(_players,

            (playerNgo) =>
            {
                Managers.VFX_Manager.GenerateParticle("Player/SkillVFX/Aura_Roar", playerNgo.transform, SkillDuration);
            }
            ,
            () =>
            {
                StatEffect effect = new StatEffect(_roarModifier.StatType, Value, _roarModifier.Buffname);
                Managers.RelayManager.NGO_RPC_Caller.Call_InitBuffer_ServerRpc(effect, Buff_IconImage_Path, SkillDuration);
            });
    }
}