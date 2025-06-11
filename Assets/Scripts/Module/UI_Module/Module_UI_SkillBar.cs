using System.Collections;
using System.Collections.Generic;
using GameManagers;
using UnityEngine;

public class Module_UI_SkillBar : MonoBehaviour
{

    void Start()
    {
        UI_SkillBar skillBar_UI = Managers.UI_Manager.GetSceneUIFromResource<UI_SkillBar>();
        Managers.SkillManager.Invoke_Done_UI_SKilBar_Init_Event();
    }

}
