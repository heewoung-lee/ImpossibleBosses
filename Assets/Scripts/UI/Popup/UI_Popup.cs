using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public abstract class UI_Popup : UI_Base
{
    protected InputAction _close_Popup_UI;
    protected override void AwakeInit()
    {
        _close_Popup_UI = Managers.InputManager.GetInputAction(Define.ControllerType.UI, "Close_Popup_UI");
        _close_Popup_UI.Enable();
    }

    protected override void OnEnableInit()
    {
        base.OnEnableInit();
        _close_Popup_UI.performed += ClosePopupUI;
    }

    protected override void OnDisableInit()
    {
        base.OnDisableInit();
        _close_Popup_UI.performed -= ClosePopupUI;
    }

    public void ClosePopupUI(InputAction.CallbackContext context)
    {
        if (Managers.UI_Manager.GetTopPopUpUI(this))
        {
            Managers.UI_Manager.ClosePopupUI();
        }
    }
}
