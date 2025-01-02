using UnityEngine;
using UnityEngine.UI;

public class UI_SkillBar : UI_Scene
{
    private Transform[] _skillContextFrames;

    public Transform[] SkillContextFrames { get => _skillContextFrames; }

    enum SkillICons
    {
        SkillContextFrame1,
        SkillContextFrame2,
        SkillContextFrame3,
        SkillContextFrame4
    }

    protected override void AwakeInit()
    {
        base.AwakeInit();
        _skillContextFrames = new Transform[4];
        Bind<Transform>(typeof(SkillICons));
        SkillICons[] skillIcons = (SkillICons[])System.Enum.GetValues(typeof(SkillICons));
        for (int i = 0; i < _skillContextFrames.Length; i++)
        {
            _skillContextFrames[i] = Get<Transform>((int)skillIcons[i]);
        }
    }

    public Transform SetLocationSkillSlot(SkillComponent skillcomponent)
    {
        foreach(Transform skillFrameTr in _skillContextFrames)
        {
            if(skillFrameTr.childCount <= 0)
            {
                return skillFrameTr;
            }
        }
        Debug.LogError("Skill slots are full.");
        return null;
    }

    protected override void StartInit()
    {

    }
}