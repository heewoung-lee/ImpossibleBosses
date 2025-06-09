public interface IPopupHandler
{
    public bool IsVisible { get; }
    public void ShowPopup();
    public void ClosePopup();
}