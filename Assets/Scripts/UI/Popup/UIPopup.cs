using GameManagers;
using UnityEngine.InputSystem;
using Util;
using Zenject;

namespace UI.Popup
{
    public abstract class UIPopup : UIBase
    {
        protected InputAction _closePopupUI;
        [Inject] private UIManager _uiManager;

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
            if (_uiManager.GetTopPopUpUI(this))
            {
                _uiManager.ClosePopupUI();
            }
        }
    }
}
