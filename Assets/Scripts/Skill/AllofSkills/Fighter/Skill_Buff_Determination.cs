using UnityEngine;

public class Skill_Buff_Determination : Skill_Duration
{
    public Skill_Buff_Determination()
    {
        _buffIconImage = Managers.ResourceManager.Load<Sprite>("Art/Player/SkillICon/WarriorSkill/BuffSkillIcon/IconSet_Equip_Helmet");
        _determination = new Buffer_Determination(_buffIconImage);
    }

    private Sprite _buffIconImage;
    private Buffer_Determination _determination;
    private Collider[] _players = null;
    private BaseController _playerBaseController;
    private Module_Fighter_Class _fighter_Class;

    public override float SkillDuration => 10f;
    public override Sprite BuffIconImage => _buffIconImage;
    public override Define.PlayerClass PlayerClass => Define.PlayerClass.Fighter;
    public override string SkillName => "����";

    public override float CoolTime => 5f;

    public override string EffectDescriptionText => $"��Ƽ���鿡�� 10�� ������ �ο��մϴ�";

    public override string ETCDescriptionText => "���ΰ��� ����";

    public override Sprite SkillconImage => Managers.ResourceManager.Load<Sprite>("Art/Player/SkillICon/WarriorSkill/SkillIcon/Determination");

    public override float Value => 10f;

    public override Buff_Modifier Buff_Modifier => _determination;

    public override void InvokeSkill()
    {
        if (_playerBaseController == null || _fighter_Class == null)
        {
            _playerBaseController = Managers.GameManagerEx.Player.GetComponent<BaseController>();
            _fighter_Class = _playerBaseController.GetComponent<Module_Fighter_Class>();
        }

        _playerBaseController.CurrentStateType = _fighter_Class.RoarState;//�ξ� �ִϸ��̼� �״�� ���

        LayerMask playerLayerMask = LayerMask.GetMask("Player");
        float skillRadius = float.MaxValue;

        _players = Physics.OverlapSphere(_playerBaseController.transform.position, skillRadius, playerLayerMask);
        foreach (Collider players_collider in _players)
        {
            if (players_collider.TryGetComponent(out BaseStats playerStats))
            {
                GameObject roarParticle = Managers.VFX_Manager.GenerateParticle("Player/SkillVFX/Shield_Determination", playerStats.gameObject, SkillDuration);
                Managers.BufferManager.InitBuff(playerStats, SkillDuration, _determination, Value);
            }
        }


    }

    public override void RemoveStats()
    {
        //�����Ǿ����� �߰� ����
    }
}
