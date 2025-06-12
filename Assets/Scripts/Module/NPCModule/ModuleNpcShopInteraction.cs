using GameManagers;
using Module.CommonModule;
using Module.PlayerModule;
using UnityEngine;

namespace Module.NPCModule
{
    public class ModuleNpcShopInteraction : MonoBehaviour, IInteraction
    {
        UI_Shop _uiShop;
        CapsuleCollider _collider;

        public bool CanInteraction => true;

        public string InteractionName => "상인";

        public Color InteractionNameColor => Color.white;

        private void Awake()
        {
            _collider = GetComponent<CapsuleCollider>();
        }
        private void Start()
        {
            _uiShop = Managers.UIManager.GetPopupUIFromResource<UI_Shop>();
            Managers.UIManager.ClosePopupUI(_uiShop);
        }
        public void Interaction(ModulePlayerInteraction caller)
        {
            Managers.UIManager.SwitchPopUpUI(_uiShop);
        }
        public void OutInteraction()
        {
            Managers.UIManager.ClosePopupUI(_uiShop);
        }

    }
}