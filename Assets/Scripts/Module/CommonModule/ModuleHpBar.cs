using GameManagers;
using UI.WorldSpace;
using UnityEngine;
using Zenject;

namespace Module.CommonModule
{
    public class ModuleHpBar : MonoBehaviour
    {
        [Inject] private UIManager _uiManager;

        void Start()
        {
            UIHpBar playerInfoUI = _uiManager.MakeUIWorldSpaceUI<UIHpBar>();
            playerInfoUI.transform.SetParent(transform);
        }
    }
}
