using Controller;
using Controller.ControllerStats;
using GameManagers;
using Module.PlayerModule.PlayerClassModule;
using Skill.BaseSkill;
using UnityEngine;
using Util;

namespace Skill.AllofSkills.Fighter
{
    public class SkillTaunt : SkillImmedialty
    {

        private const float DurationParticle = 5f;
        private BaseController _playerController;
        private ModuleFighterClass _fighterClass;
        private Collider[] _monsters;

        public override Define.PlayerClass PlayerClass => Define.PlayerClass.Fighter;
        public override string SkillName => "도발";
        public override float CoolTime => 10f;
        public override string EffectDescriptionText => $"적들에게 도발을해 나를 i아오도록한다";
        public override string ETCDescriptionText => "메롱";
        public override Sprite SkillconImage => Managers.ResourceManager.Load<Sprite>("Art/Player/SkillICon/WarriorSkill/SkillIcon/Taunt");
        public override float Value => 0f;
        public override BaseController PlayerController { 
            get => _playerController;
            protected set => _playerController = value;
        }
        public override ModulePlayerClass ModulePlayerClass {
            get => _fighterClass;
            protected set => _fighterClass = value as ModuleFighterClass;
        }
        public override IState State => _fighterClass.TauntState;
        public override void InvokeSkill()
        {
            base.InvokeSkill();
        }

        public override void AddInitailzeState()
        {
            base.AddInitailzeState();
            _fighterClass.TauntState.UpdateStateEvent += PlaytheTaunt;
        }
        public void PlaytheTaunt()
        {
            foreach(Collider monster in _monsters)
            {
                if (monster.TryGetComponent(out BaseController controller))
                {
                    controller.TargetObject = _playerController.gameObject;
                }
            }
        }

        public override void SkillAction()
        {
            Managers.VFXManager.GenerateParticle("Prefabs/Player/SkillVFX/Taunt_Player", _playerController.transform, DurationParticle);
            _monsters = Managers.BufferManager.DetectedOther("Monster");
            foreach (Collider monster in _monsters)
            {
                HeadTr headTr = monster.GetComponentInChildren<HeadTr>();
                if (headTr != null)
                {
                    Managers.VFXManager.GenerateParticle("Prefabs/Player/SkillVFX/Taunt_Enemy", headTr.transform, DurationParticle);
                }
            }
        }
    }
}