using GameManagers;
using UnityEngine;

namespace Module.CommonModule
{
    public class ModulePortalIndicator : MonoBehaviour
    {
        void Start()
        {
            UI_PortalIndicator uiPortalIndicator = Managers.UIManager.MakeUIWorldSpaceUI<UI_PortalIndicator>();
            uiPortalIndicator.transform.SetParent(transform);
        }
    }
}
