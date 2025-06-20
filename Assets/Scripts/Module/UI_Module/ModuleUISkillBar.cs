using GameManagers;
using GameManagers.Interface;
using UI.Scene.SceneUI;
using UnityEngine;
using Zenject;

namespace Module.UI_Module
{
    public class ModuleUISkillBar : MonoBehaviour
    {
        [Inject]IUISceneManager _uiManager;
        [Inject]SkillManager _skillManager; 
        
        void Start()
        {
            UISkillBar skillBarUI = _uiManager.GetSceneUIFromResource<UISkillBar>();
            _skillManager.Invoke_Done_UI_SKilBar_Init_Event();
        }

    }
}
