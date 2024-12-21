using UnityEngine;
using UnityEngine.UI;

public class UI_SkillBar : UI_Scene
{
    private Image[] _skillICon;

    enum SkillICons
    {
        Skill1icon,
        Skill2icon,
        Skill3icon,
        Skill4icon
    }

    protected override void AwakeInit()
    {
        base.AwakeInit();
        _skillICon = new Image[4];
        Bind<Image>(typeof(SkillICons));
        SkillICons[] skillIcons = (SkillICons[])System.Enum.GetValues(typeof(SkillICons));
        for (int i = 0; i < _skillICon.Length; i++)
        {
            _skillICon[i] = Get<Image>((int)skillIcons[i]);
        }
    }

    protected override void StartInit()
    {

    }
}