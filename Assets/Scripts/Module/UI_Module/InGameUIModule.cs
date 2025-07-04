using GameManagers;
using UnityEngine;
using Zenject;

namespace Module.UI_Module
{
    public class InGameUIModule : MonoBehaviour
    {
        [Inject] private NgoPoolManager _poolManager;
        private void Start()
        {
            StartInit();
        }

        protected virtual void StartInit()
        {
            gameObject.AddComponent<ModuleUIBufferBar>();
            gameObject.AddComponent<ModuleUIConsumableBar>();
            gameObject.AddComponent<ModuleUIItemDragImage>();
            gameObject.AddComponent<ModuleUIPlayerInventory>();
            gameObject.AddComponent<ModuleUIPlayerInfo>();
            gameObject.AddComponent<ModuleUISkillBar>();
            gameObject.AddComponent<ModuleUIDescription>();

            _poolManager.Create_NGO_Pooling_Object();

        }
    }
}
