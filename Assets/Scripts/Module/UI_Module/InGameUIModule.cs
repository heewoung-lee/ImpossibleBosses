using GameManagers;
using GameManagers.Interface.ResourcesManager;
using UnityEngine;
using Zenject;

namespace Module.UI_Module
{
    public class InGameUIModule : MonoBehaviour
    {
        [Inject] private NgoPoolManager _poolManager;
        [Inject] IInstantiate _instantiator;
        private void Start()
        {
            StartInit();
        }

        protected virtual void StartInit()
        {
            _instantiator.GetOrAddComponent<ModuleUIBufferBar>(gameObject);
            _instantiator.GetOrAddComponent<ModuleUIConsumableBar>(gameObject);
            _instantiator.GetOrAddComponent<ModuleUIItemDragImage>(gameObject);
            _instantiator.GetOrAddComponent<ModuleUIPlayerInventory>(gameObject);
            _instantiator.GetOrAddComponent<ModuleUIPlayerInfo>(gameObject);
            _instantiator.GetOrAddComponent<ModuleUISkillBar>(gameObject);
            _instantiator.GetOrAddComponent<ModuleUIDescription>(gameObject);

            _poolManager.Create_NGO_Pooling_Object();

        }
    }
}
