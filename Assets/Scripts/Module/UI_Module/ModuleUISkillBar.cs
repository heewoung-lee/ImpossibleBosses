using GameManagers;
using UnityEngine;
using Zenject;

namespace Module.UI_Module
{
    public class ModuleUISkillBar : MonoBehaviour
    {
        [Inject]UIManager _uiManager;
        void Start()
        {
            UI_SkillBar skillBarUI = _uiManager.GetSceneUIFromResource<UI_SkillBar>();
            Managers.SkillManager.Invoke_Done_UI_SKilBar_Init_Event();
        }

    }
}
