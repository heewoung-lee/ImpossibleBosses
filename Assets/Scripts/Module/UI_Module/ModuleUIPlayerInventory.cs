using GameManagers;
using UI.Popup.PopupUI;
using UnityEngine;
using UnityEngine.InputSystem;
using Util;
using Zenject;

namespace Module.UI_Module
{
    public class ModuleUIPlayerInventory : MonoBehaviour
    {

        private UIPlayerInventory _inventoryUI;
        private InputAction _switchInventoryUI;
        [Inject] private UIManager _uiManager;
        private void Awake()
        {
            _switchInventoryUI = Managers.InputManager.GetInputAction(Define.ControllerType.UI, "Show_UI_Inventory");
            _switchInventoryUI.Enable();
            if (Managers.GameManagerEx.Player == null)
            {
                Managers.GameManagerEx.OnPlayerSpawnEvent += (playerStats) => InitalizeInventoryKey();
            }
            else
            {
                InitalizeInventoryKey();
            }
            void InitalizeInventoryKey()
            {
                _inventoryUI = _uiManager.GetPopupUIFromResource<UIPlayerInventory>();
            }
        }

        private void OnEnable()
        {
            _switchInventoryUI.performed += Switch_Inventoy;
        }

        private void OnDisable()
        {
            _switchInventoryUI.performed -= Switch_Inventoy;
        }

        public void Switch_Inventoy(InputAction.CallbackContext context)
        {
            _uiManager.SwitchPopUpUI(_inventoryUI);
        }
    }
}