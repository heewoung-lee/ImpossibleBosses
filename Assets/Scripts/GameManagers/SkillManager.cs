using System;
using System.Collections.Generic;
using GameManagers.Interface;
using Skill.BaseSkill;
using Util;
using Zenject;

namespace GameManagers
{
    public class SkillManager : IManagerInitializable,IManagerIResettable
    {
        
        Dictionary<string, BaseSkill> _allSKillDict = new Dictionary<string, BaseSkill>();
        public Dictionary<string, BaseSkill> AllSKillDict { get => _allSKillDict; }
        [Inject] public IUISceneManager SceneManager { get; }

        private Action _doneUISkillBarInitEvent;

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



        List<Type> _skillType = new List<Type>();

        private UI_SkillBar _uiSkillBar;
        public UI_SkillBar UISkillBar
        {
            get
            {
                if (_uiSkillBar == null)
                {
                    if(SceneManager.Try_Get_Scene_UI(out UI_SkillBar skillbar))
                    {
                        _uiSkillBar = skillbar;
                    }
                }
                return _uiSkillBar;
            }
        }
        public void Init()
        {
            //Skill/AllofSkill에 있는 타입들을 가져온다.
            _skillType = Managers.DataManager.LoadSerializableTypesFromFolder("Assets/Scripts/Skill/AllofSkills", GetAllofSkill);
            foreach (Type type in _skillType)
            {
           
                BaseSkill skill = Activator.CreateInstance(type) as BaseSkill;

                AllSKillDict.Add(skill.SkillName, skill);
            }

        }


        private void GetAllofSkill(Type type, List<Type> typeList)
        {
            if (typeof(BaseSkill).IsAssignableFrom(type))
            {
                typeList.Add(type);
            }
        }

        public void Invoke_Done_UI_SKilBar_Init_Event()
        {
            _doneUISkillBarInitEvent?.Invoke();
        }

        public void Clear()
        {
            _doneUISkillBarInitEvent = null;
        }
    }
}