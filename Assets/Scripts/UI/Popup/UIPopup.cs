using GameManagers;
using UnityEngine.InputSystem;
using Util;

namespace UI.Popup
{
    public abstract class UIPopup : UIBase
    {
        protected InputAction _closePopupUI;
        protected override void AwakeInit()
        {
            _closePopupUI = Managers.InputManager.GetInputAction(Define.ControllerType.UI, "Close_Popup_UI");
            _closePopupUI.Enable();
        }

        protected override void OnEnableInit()
        {
            base.OnEnableInit();
            _closePopupUI.performed += ClosePopupUI;
        }

        protected override void OnDisableInit()
        {
            base.OnDisableInit();
            _closePopupUI.performed -= ClosePopupUI;
        }

        public void ClosePopupUI(InputAction.CallbackContext context)
        {
            if (Managers.UIManager.GetTopPopUpUI(this))
            {
                Managers.UIManager.ClosePopupUI();
            }
        }
    }
}
