using UnityEngine;
using static PlaySceneMockUnitTest;

public class Skill_Buff_Determination : Skill_Duration
{
    public Skill_Buff_Determination()
    {
        _buffIconImage = Managers.ResourceManager.Load<Sprite>("Art/Player/SkillICon/WarriorSkill/BuffSkillIcon/IconSet_Equip_Helmet");
        _determination = new Buffer_Determination(_buffIconImage);
    }
    //TODO: ���� ������ �̹��� �ٲܰ�
    private Sprite _buffIconImage;
    private Buffer_Determination _determination;
    private Collider[] _players = null;
    private BaseController _playerController;
    private Module_Fighter_Class _fighter_Class;
    public override string Buff_IconImage_Path => "Art/Player/SkillICon/WarriorSkill/SkillIcon/Determination";
    public override float SkillDuration => 10f;
    public override Sprite BuffIconImage => _buffIconImage;
    public override Define.PlayerClass PlayerClass => Define.PlayerClass.Fighter;
    public override string SkillName => "����";
    public override float CoolTime => 5f;
    public override string EffectDescriptionText => $"��Ƽ���鿡�� 10�� ������ �ο��մϴ�";
    public override string ETCDescriptionText => "���ΰ��� ����";
    public override Sprite SkillconImage => Managers.ResourceManager.Load<Sprite>(Buff_IconImage_Path);
    public override float Value => 10f;
    public override Buff_Modifier Buff_Modifier => _determination;
    public override BaseController PlayerController { 
        get => _playerController;
        protected set => _playerController = value; }
    public override Module_Player_Class Module_Player_Class { 
        get => _fighter_Class;
        protected set => _fighter_Class = value as Module_Fighter_Class;  }
    public override IState state => _fighter_Class.DeterminationState;


    public override void InvokeSkill()
    {
        base.InvokeSkill();
    }
    public override void RemoveStats()
    {
        //�����Ǿ����� �߰� ����
    }

    public override void SkillAction()
    {
        _players = Managers.BufferManager.DetectedPlayers();

        Managers.BufferManager.ALL_Character_ApplyBuffAndCreateParticle(_players,
        (playerNgo) =>
        {
                Managers.VFX_Manager.GenerateParticle("Player/SkillVFX/Shield_Determination", playerNgo.transform, SkillDuration);
            }
            ,
            () =>
            {
                StatEffect effect = new StatEffect(_determination.StatType, Value, _determination.Buffname);
                Managers.RelayManager.NGO_RPC_Caller.Call_InitBuffer_ServerRpc(effect, Buff_IconImage_Path, SkillDuration);
            });
    }
}
