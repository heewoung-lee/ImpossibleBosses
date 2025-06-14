using GameManagers;
using UI.WorldSpace;
using UnityEngine;

namespace Module.CommonModule
{
    public class ModulePortalIndicator : MonoBehaviour
    {
        void Start()
        {
            UIPortalIndicator uiPortalIndicator = Managers.UIManager.MakeUIWorldSpaceUI<UIPortalIndicator>();
            uiPortalIndicator.transform.SetParent(transform);
        }
    }
}
