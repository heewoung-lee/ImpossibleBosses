using BehaviorDesigner.Runtime.Tasks.Unity.UnityInput;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UI_SkillBar : UI_Scene
{
    private Transform[] _skillContextFrames;
    private InputAction _getQKey;
    private InputAction _getWKey;
    private InputAction _getEKey;
    private InputAction _getRKey;

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
        BindKeyBoard();
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

    public void BindKeyBoard()
    {
        _getQKey = Managers.InputManager.GetInputAction(Define.ControllerType.UI, "SkillBar_GetKeyQ");
        _getWKey = Managers.InputManager.GetInputAction(Define.ControllerType.UI, "SkillBar_GetKeyW");
        _getEKey = Managers.InputManager.GetInputAction(Define.ControllerType.UI, "SkillBar_GetKeyE");
        _getRKey = Managers.InputManager.GetInputAction(Define.ControllerType.UI, "SkillBar_GetKeyR");

        _getQKey.Enable();
        _getWKey.Enable();
        _getEKey.Enable();
        _getRKey.Enable();

        _getQKey.started += GetQKey;
    }

    public void GetQKey(InputAction.CallbackContext context)
    {
        
    }


    protected override void StartInit()
    {

    }
}