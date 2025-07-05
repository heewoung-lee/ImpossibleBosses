using GameManagers;
using GameManagers.Interface;
using UI.WorldSpace;
using UnityEngine;
using Zenject;

namespace Module.CommonModule
{
    public class ModuleHpBar : MonoBehaviour
    {
        [Inject] private IUISubItem _uiSubItemManager;

        void Start()
        {
            UIHpBar playerInfoUI = _uiSubItemManager.MakeUIWorldSpaceUI<UIHpBar>();
            playerInfoUI.transform.SetParent(transform);
        }
    }
}
