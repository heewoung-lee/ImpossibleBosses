using BehaviorDesigner.Runtime.Tasks.Unity.UnityLayerMask;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityLight;
using Google.Apis.Sheets.v4.Data;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class Skill_Buff_Roar : Skill_Duration
{
    public const string BUFFICONIAMGE_PATH = "Art/Player/SkillICon/WarriorSkill/BuffSkillIcon/Icon_Booster_Power";
    public Skill_Buff_Roar()
    {
        _buffIconImage =  Managers.ResourceManager.Load<Sprite>(BUFFICONIAMGE_PATH);
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
        _players = Managers.BufferManager.DetectedPlayers();

        Managers.BufferManager.ALL_Character_ApplyBuffAndCreateParticle(_players,

            (playerNgo) =>
            {
                Managers.VFX_Manager.GenerateParticle("Player/SkillVFX/Aura_Roar", playerNgo.transform, SkillDuration);
            }
            ,
            () =>{
                StatEffect effect = new StatEffect(_roarModifier.StatType, Value, _roarModifier.Buffname);
                Managers.RelayManager.NGO_RPC_Caller.Call_InitBuffer_ServerRpc(effect, BUFFICONIAMGE_PATH, SkillDuration);
            });
    }



    public override void RemoveStats()
    {
        if (_players != null)
        {

        }
    }

}