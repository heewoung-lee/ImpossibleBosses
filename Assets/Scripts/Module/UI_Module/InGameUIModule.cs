using GameManagers;
using UnityEngine;

namespace Module.UI_Module
{
    public class InGameUIModule : MonoBehaviour
    {

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

            Managers.NgoPoolManager.Create_NGO_Pooling_Object();

        }
    }
}
