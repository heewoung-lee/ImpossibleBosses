using GameManagers;
using UnityEngine;
using Zenject;

namespace Module.UI_Module
{
    public class ModuleUIConsumableBar : MonoBehaviour
    {
        [Inject]private UIManager _uiManager; 
        void Start()
        {
            UI_ConsumableBar uiConsumableBar = _uiManager.GetSceneUIFromResource<UI_ConsumableBar>();
        }

    }
}
