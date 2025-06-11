using GameManagers;
using UnityEngine;
using UnityEngine.InputSystem;

public class Module_NPC_SHOP_Interaction : MonoBehaviour, IInteraction
{
    UI_Shop _ui_shop;
    CapsuleCollider _collider;

    public bool CanInteraction => true;

    public string InteractionName => "상인";

    public Color InteractionNameColor => Color.white;

    private void Awake()
    {
        _collider = GetComponent<CapsuleCollider>();
    }
    private void Start()
    {
        _ui_shop = Managers.UI_Manager.GetPopupUIFromResource<UI_Shop>();
        Managers.UI_Manager.ClosePopupUI(_ui_shop);
    }
    public void Interaction(Module_Player_Interaction caller)
    {
        Managers.UI_Manager.SwitchPopUpUI(_ui_shop);
    }
    public void OutInteraction()
    {
        Managers.UI_Manager.ClosePopupUI(_ui_shop);
    }

}