using GameManagers;
using GameManagers.Interface;
using GameManagers.Interface.InputManager;
using GameManagers.Interface.UIManager;
using UnityEngine.InputSystem;
using Util;
using Zenject;

namespace UI.Popup
{
    public abstract class UIPopup : UIBase
    {
        protected InputAction _closePopupUI;
        [Inject] private IUIPopupManager _popupUIManager;
        [Inject] private IInputAsset _inputManager;

        protected override void AwakeInit()
        {
            _closePopupUI = _inputManager.GetInputAction(Define.ControllerType.UI, "Close_Popup_UI");
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
            if (_popupUIManager.GetTopPopUpUI(this))
            {
                _popupUIManager.ClosePopupUI();
            }
        }
    }
}
