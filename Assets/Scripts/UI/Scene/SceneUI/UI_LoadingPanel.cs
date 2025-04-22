using UnityEngine;
using UnityEngine.UI;

public class UI_LoadingPanel : UI_Scene
{
    enum LoadingPanel
    {
        LoadingPanel
    }
    private GameObject _loadingPanel;
    private Image _loadingPanelImage;
    protected override void AwakeInit()
    {
        base.AwakeInit();
        Bind<GameObject>(typeof(LoadingPanel));
        _loadingPanel = Get<GameObject>(((int)LoadingPanel.LoadingPanel));
        _loadingPanelImage = _loadingPanel.GetComponentInChildren<Image>();
        Managers.LobbyManager.LobbyLoadingEvent += LobbyLoading;
    }


    protected override void StartInit()
    {
        base.StartInit();
        _loadingPanelImage.enabled = false;
        SetSortingOrder(100);
    }

    public void LobbyLoading(bool isLobbyLoading)
    {
        if(_loadingPanel != null)
        {
            _loadingPanelImage.ImageEnable(isLobbyLoading);
        }
    }
}
