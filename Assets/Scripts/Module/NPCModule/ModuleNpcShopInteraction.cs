using GameManagers;
using Module.CommonModule;
using Module.PlayerModule;
using UI.Popup.PopupUI;
using UnityEngine;
using Zenject;

namespace Module.NPCModule
{
    public class ModuleNpcShopInteraction : MonoBehaviour, IInteraction
    {
        private UIShop _uiShop;
        private CapsuleCollider _collider;
        [Inject] private UIManager _uiManager;


        public bool CanInteraction => true;

        public string InteractionName => "상인";

        public Color InteractionNameColor => Color.white;

        private void Awake()
        {
            _collider = GetComponent<CapsuleCollider>();
        }
        private void Start()
        {
            _uiShop = _uiManager.GetPopupUIFromResource<UIShop>();
            _uiManager.ClosePopupUI(_uiShop);
        }
        public void Interaction(ModulePlayerInteraction caller)
        {
            _uiManager.SwitchPopUpUI(_uiShop);
        }
        public void OutInteraction()
        {
            _uiManager.ClosePopupUI(_uiShop);
        }

    }
}