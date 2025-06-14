using GameManagers;
using UI.Popup.PopupUI;
using UnityEngine;
using UnityEngine.InputSystem;
using Util;

namespace Module.UI_Module
{
    public class ModuleUIPlayerInventory : MonoBehaviour
    {

        private UIPlayerInventory _inventoryUI;
        private InputAction _switchInventoryUI;
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
                _inventoryUI = Managers.UIManager.GetPopupUIFromResource<UIPlayerInventory>();
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
            Managers.UIManager.SwitchPopUpUI(_inventoryUI);
        
        }
    }
}