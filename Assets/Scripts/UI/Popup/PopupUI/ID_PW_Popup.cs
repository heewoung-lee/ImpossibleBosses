using TMPro;
using UnityEngine.InputSystem;

public abstract class ID_PW_Popup : UI_Popup
{
    InputAction _inputTabKey;

    public abstract TMP_InputField ID_Input_Field { get; }
    public abstract TMP_InputField PW_Input_Field { get; }


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
            PW_Input_Field.ActivateInputField();
        }
        else
        {
            ID_Input_Field.ActivateInputField();
        }
    }
}