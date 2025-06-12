using GameManagers;
using UnityEngine;

namespace Module.UI_Module
{
    public class ModuleUISkillBar : MonoBehaviour
    {

        void Start()
        {
            UI_SkillBar skillBarUI = Managers.UIManager.GetSceneUIFromResource<UI_SkillBar>();
            Managers.SkillManager.Invoke_Done_UI_SKilBar_Init_Event();
        }

    }
}
