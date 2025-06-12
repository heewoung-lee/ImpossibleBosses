using GameManagers;
using UnityEngine;

namespace Module.UI_Module
{
    public class ModuleUIPlayerTestButton : MonoBehaviour
    {

        void Start()
        {
            UI_CREATE_ITEM_AND_GOLD_Button buttonUI = Managers.UIManager.GetSceneUIFromResource<UI_CREATE_ITEM_AND_GOLD_Button>();
        }

    }
}
