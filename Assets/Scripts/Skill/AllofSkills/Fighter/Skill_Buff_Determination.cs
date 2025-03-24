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
    private BaseController _playerController;
    private Module_Fighter_Class _fighter_Class;

    public override float SkillDuration => 10f;
    public override Sprite BuffIconImage => _buffIconImage;
    public override Define.PlayerClass PlayerClass => Define.PlayerClass.Fighter;
    public override string SkillName => "결의";

    public override float CoolTime => 5f;

    public override string EffectDescriptionText => $"파티원들에게 10의 방어력을 부여합니다";

    public override string ETCDescriptionText => "서로간의 결의";

    public override Sprite SkillconImage => Managers.ResourceManager.Load<Sprite>("Art/Player/SkillICon/WarriorSkill/SkillIcon/Determination");

    public override float Value => 10f;

    public override Buff_Modifier Buff_Modifier => _determination;

    public override void InvokeSkill()
    {
        if (_playerController == null || _fighter_Class == null)
        {
            _playerController = Managers.GameManagerEx.Player.GetComponent<BaseController>();
            _fighter_Class = _playerController.GetComponent<Module_Fighter_Class>();
            _playerController.StateAnimDict.RegisterState(_fighter_Class.DeterminationState, OnStateChanged);

        }
        _playerController.CurrentStateType = _fighter_Class.DeterminationState;
    }

    public void OnStateChanged()
    {
        LayerMask playerLayerMask = LayerMask.GetMask("Player");
        float skillRadius = float.MaxValue;
        _players = Physics.OverlapSphere(_playerController.transform.position, skillRadius, playerLayerMask);
        foreach (Collider players_collider in _players)
        {
            if (players_collider.TryGetComponent(out BaseStats playerStats))
            {
                GameObject determinationParticle = Managers.VFX_Manager.GenerateParticle("Player/SkillVFX/Shield_Determination", playerStats.transform, SkillDuration);
                Managers.BufferManager.InitBuff(playerStats, SkillDuration, _determination, Value);
            }
        }
    }

    public override void RemoveStats()
    {
        //해제되었을때 추가 로직
    }
}
