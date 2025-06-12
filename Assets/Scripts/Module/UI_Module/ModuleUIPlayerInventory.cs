using GameManagers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Module.UI_Module
{
    public class ModuleUIPlayerInventory : MonoBehaviour
    {

        private UI_Player_Inventory _inventoryUI;
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
                _inventoryUI = Managers.UIManager.GetPopupUIFromResource<UI_Player_Inventory>();
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