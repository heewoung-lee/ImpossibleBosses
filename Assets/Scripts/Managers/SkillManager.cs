using System;
using System.Collections.Generic;

public class SkillManager : IManagerInitializable
{
    Dictionary<string, IBaseSkill> _allSKillDict = new Dictionary<string, IBaseSkill>();

    List<Type> _requestType = new List<Type>();

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
        throw new System.NotImplementedException();
    }
}