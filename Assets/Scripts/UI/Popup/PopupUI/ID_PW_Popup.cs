using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class ID_PW_Popup : UI_Popup
{
    private InputAction _inputTabKey;

    public abstract TMP_InputField ID_Input_Field { get; }
    public abstract TMP_InputField PW_Input_Field { get; }


    protected Action<TMP_InputField> _inputAction;

    protected override void AwakeInit()
    {
        base.AwakeInit();
        _inputTabKey = Managers.InputManager.GetInputAction(Define.ControllerType.UI, "ID_PW_Popup_TabKey");
        _inputTabKey.Enable();

        _inputTabKey.started += SwitchingField;
    }

    private void SwitchingField(InputAction.CallbackContext context)
    {
        if (ID_Input_Field.isFocused)
        {
            Debug.Log("IDÅÇÅ°´­¸²");
            PW_Input_Field.ActivateInputField();
            _inputAction?.Invoke(ID_Input_Field);
        }
        else
        {
            Debug.Log("PWÅÇÅ°´­¸²");
            ID_Input_Field.ActivateInputField();
        }
    }
}