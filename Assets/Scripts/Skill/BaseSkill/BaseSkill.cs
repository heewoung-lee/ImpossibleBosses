using Controller;
using Controller.ControllerStats;
using GameManagers;
using Module.PlayerModule.PlayerClassModule;
using UnityEngine;
using Util;
using Zenject;

namespace Skill.BaseSkill
{
    public abstract class BaseSkill
    {
        [Inject] GameManagerEx _gameManagerEx;
        public abstract Define.PlayerClass PlayerClass { get; }
        public abstract string SkillName { get; }
        public abstract float CoolTime {  get; }
        public abstract string EffectDescriptionText { get; }
        public abstract string ETCDescriptionText { get; }
        public abstract Sprite SkillconImage { get; }
        public abstract float Value { get; }

        public virtual void AddInitailzeState() { }
        public abstract BaseController PlayerController { get; protected set; }
        public abstract ModulePlayerClass ModulePlayerClass { get; protected set; }

        public abstract void SkillAction();

        public abstract IState State { get; }


        public virtual void InvokeSkill()
        {
            if (PlayerController == null || ModulePlayerClass == null)
            {
                PlayerController = _gameManagerEx.Player.GetComponent<BaseController>();
                ModulePlayerClass = PlayerController.GetComponent<ModulePlayerClass>();
                PlayerController.StateAnimDict.RegisterState(State, SkillAction);
                AddInitailzeState();
            }
            PlayerController.CurrentStateType = State;
        }

        private BaseController _baseController;

        public bool IsStateUpdatedAfterSkill()
        {
            if(_baseController == null)
            {
                _baseController =  _gameManagerEx.Player.GetComponent<BaseController>();
            }
            IState currentIState = _baseController.CurrentStateType;
            InvokeSkill();

            return currentIState != _baseController.CurrentStateType ? true : false;
        }

    }
}