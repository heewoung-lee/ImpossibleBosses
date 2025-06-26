using GameManagers;
using GameManagers.Interface.InputManager;
using TMPro;
using UnityEngine.InputSystem;
using Util;
using Zenject;

namespace UI.Popup.PopupUI
{
    public abstract class IDPwPopup : UIPopup
    {
        [Inject] private IInputAsset _inputManager;
        
        private InputAction _inputTabKey;
        public abstract TMP_InputField IdInputField { get; }
        public abstract TMP_InputField PwInputField { get; }

        protected override void AwakeInit()
        {
            base.AwakeInit();
            _inputTabKey = _inputManager.GetInputAction(Define.ControllerType.UI, "ID_PW_Popup_TabKey");
            _inputTabKey.Enable();

            _inputTabKey.started += SwitchingField;
        }

        protected void SwitchingField(InputAction.CallbackContext context)
        {
            if (IdInputField.isFocused)
            {
                PwInputField.ActivateInputField();
            }
            else
            {
                IdInputField.ActivateInputField();
            }
        }
    }
}