using GameManagers;
using UnityEngine;

namespace Module.UI_Module
{
    public class ModuleUIConsumableBar : MonoBehaviour
    {

        void Start()
        {
            UI_ConsumableBar uiConsumableBar = Managers.UIManager.GetSceneUIFromResource<UI_ConsumableBar>();
        }

    }
}
