using UnityEngine.UI;

interface IUI_HasCloseButton
{
    Button CloseButton { get; }
    public void OnClickCloseButton();
}