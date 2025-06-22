using GameManagers;
using GameManagers.Interface;
using GameManagers.Interface.InputManager_Interface;
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
        [Inject] private IUIPopupManager _uiManager;
        [Inject] private IInputAsset _inputManager;
        [Inject] GameManagerEx _gameManagerEx;
        private void Awake()
        {
            _switchInventoryUI = _inputManager.GetInputAction(Define.ControllerType.UI, "Show_UI_Inventory");
            _switchInventoryUI.Enable();
            if (_gameManagerEx.Player == null)
            {
                _gameManagerEx.OnPlayerSpawnEvent += (playerStats) => InitalizeInventoryKey();
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