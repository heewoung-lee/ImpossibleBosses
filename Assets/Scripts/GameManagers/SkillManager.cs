using System;
using System.Collections.Generic;
using GameManagers.Interface;
using GameManagers.Interface.DataManager;
using GameManagers.Interface.SkillManager;
using GameManagers.Interface.UI_Interface;
using GameManagers.Interface.UIManager;
using Skill.BaseSkill;
using UI.Scene.SceneUI;
using UnityEngine.SceneManagement;
using Util;
using Zenject;

namespace GameManagers
{
    public class SkillManager : IInitializable,ISkillManager
    {
        [Inject] private readonly IUISceneManager _sceneManager;
        [Inject] private IRequestDataType _requestDataType;
        Dictionary<string, BaseSkill> _allSKillDict = new Dictionary<string, BaseSkill>();
        private Action _doneUISkillBarInitEvent;
        private IList<Type> _skillType = new List<Type>();
        private UISkillBar _uiSkillBar;
        
        public IDictionary<string, BaseSkill> GetSkills()
        {
            return _allSKillDict;
        }

        public event Action DoneUISkilBarInitEvent
        {
            add
            {
                UniqueEventRegister.AddSingleEvent(ref _doneUISkillBarInitEvent,value);
            }
            remove
            {
                UniqueEventRegister.RemovedEvent(ref _doneUISkillBarInitEvent, value);
            }
        }

        public UISkillBar GetUISkillBar()
        {
            if (_uiSkillBar == null)
            {
                if(_sceneManager.Try_Get_Scene_UI(out UISkillBar skillbar))
                {
                    _uiSkillBar = skillbar;
                }
            }
            return _uiSkillBar;
        }

        public void Invoke_Done_UI_SKilBar_Init_Event()
        {
            _doneUISkillBarInitEvent?.Invoke();
        }
        public void Initialize()
        {
            //Skill/AllofSkill에 있는 타입들을 가져온다.
            _skillType = _requestDataType.LoadSerializableTypesFromFolder("Assets/Scripts/Skill/AllofSkills", GetAllofSkill);
            foreach (Type type in _skillType)
            {
           
                BaseSkill skill = Activator.CreateInstance(type) as BaseSkill;

                _allSKillDict.Add(skill.SkillName, skill);
            }

        }
        private void GetAllofSkill(Type type, List<Type> typeList)
        {
            if (typeof(BaseSkill).IsAssignableFrom(type))
            {
                typeList.Add(type);
            }
        }
    }
}