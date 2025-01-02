using System;
using System.Collections.Generic;

public class SkillManager : IManagerInitializable
{
    Dictionary<string, IBaseSkill> _allSKillDict = new Dictionary<string, IBaseSkill>();

    List<Type> _skillType = new List<Type>();

    private UI_SkillBar _ui_SkillBar;
    public UI_SkillBar UI_SkillBar
    {
        get
        {
            if (_ui_SkillBar == null)
                _ui_SkillBar = Managers.UI_Manager.Get_Scene_UI<UI_SkillBar>();

            return _ui_SkillBar;
        }
    }


    public void Init()
    {
        //Skill/AllofSkill�� �ִ� Ÿ�Ե��� �����´�.
        _skillType = Managers.DataManager.LoadSerializableTypesFromFolder("Assets/Scripts/Skill/AllofSkills", GetAllofSkill);
        foreach (Type type in _skillType)
        {
            // Activator.CreateInstance�� �ν��Ͻ� ���� _requestType�� ��Ÿ������ �̹Ƿ� �ν��Ͻ��� �ƴ�
            //���� Type ��Ÿ������ �������� �ν��Ͻ��� �����������

            IBaseSkill skill = Activator.CreateInstance(type) as IBaseSkill;

            _allSKillDict.Add(skill.SkillName, skill);
        }

    }


    private void GetAllofSkill(Type type, List<Type> typeList)
    {
        if (typeof(IBaseSkill).IsAssignableFrom(type))
        {
            typeList.Add(type);
        }
    }
}