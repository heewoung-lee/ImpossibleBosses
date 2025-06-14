using GameManagers;
using UI.WorldSpace;
using UnityEngine;

namespace Module.CommonModule
{
    public class ModuleHpBar : MonoBehaviour
    {
        void Start()
        {
            UIHpBar playerInfoUI = Managers.UIManager.MakeUIWorldSpaceUI<UIHpBar>();
            playerInfoUI.transform.SetParent(transform);
        }
    }
}
