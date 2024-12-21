using UnityEngine;
using UnityEngine.InputSystem;

public class Module_UI_Player_Inventory : MonoBehaviour
{

    private UI_Player_Inventory _inventory_UI;
    private InputAction _switch_Inventory_UI;
    private void Awake()
    {
        _switch_Inventory_UI = Managers.InputManager.GetInputAction(Define.ControllerType.UI, "Show_UI_Inventory");
        _switch_Inventory_UI.Enable();
        _inventory_UI = Managers.UI_Manager.ShowUIPopupUI<UI_Player_Inventory>();
    }

    private void OnEnable()
    {
        _switch_Inventory_UI.performed += Switch_Inventoy;
    }

    private void OnDisable()
    {
        _switch_Inventory_UI.performed -= Switch_Inventoy;
    }
    private void Start()
    {
        Managers.UI_Manager.ClosePopupUI(_inventory_UI);
    }

    public void Switch_Inventoy(InputAction.CallbackContext context)
    {
        Managers.UI_Manager.SwitchPopUpUI(_inventory_UI);
    }
}