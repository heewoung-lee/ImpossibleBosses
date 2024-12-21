using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Module_UI_SkillBar : MonoBehaviour
{

    void Start()
    {
        UI_SkillBar skillBar_UI = Managers.UI_Manager.ShowSceneUI<UI_SkillBar>();
    }

}
