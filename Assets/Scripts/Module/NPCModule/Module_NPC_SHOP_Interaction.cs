using UnityEngine;
using UnityEngine.InputSystem;

public class Module_NPC_SHOP_Interaction : MonoBehaviour, IInteraction
{
    UI_Shop _ui_shop;
    private void Start()
    {
        _ui_shop = Managers.UI_Manager.ShowUIPopupUI<UI_Shop>();
        Managers.UI_Manager.ClosePopupUI(_ui_shop);
    }
    public void Interaction()
    {
        Managers.UI_Manager.SwitchPopUpUI(_ui_shop);
    }
}