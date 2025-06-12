using GameManagers;
using UnityEngine;

namespace Module.CommonModule
{
    public class ModuleHpBar : MonoBehaviour
    {
        void Start()
        {
            UI_HPBar playerInfoUI = Managers.UIManager.MakeUIWorldSpaceUI<UI_HPBar>();
            playerInfoUI.transform.SetParent(transform);
        }
    }
}
