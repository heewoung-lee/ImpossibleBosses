using GameManagers;
using UI.WorldSpace;
using UnityEngine;
using Zenject;

namespace Module.CommonModule
{
    public class ModulePortalIndicator : MonoBehaviour
    {
        [Inject] private UIManager _uiManager;
        void Start()
        {
            UIPortalIndicator uiPortalIndicator = _uiManager.MakeUIWorldSpaceUI<UIPortalIndicator>();
            uiPortalIndicator.transform.SetParent(transform);
        }
    }
}
