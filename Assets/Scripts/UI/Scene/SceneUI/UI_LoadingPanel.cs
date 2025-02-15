using UnityEngine;

public class UI_LoadingPanel : UI_Scene
{
    enum LoadingPanel
    {
        LoadingPanel
    }
    private GameObject _loadingPanel;
    protected override void AwakeInit()
    {
        base.AwakeInit();
        Bind<GameObject>(typeof(LoadingPanel));
        _loadingPanel = Get<GameObject>(((int)LoadingPanel.LoadingPanel));
        Managers.LobbyManager.LobbyLoading -= LobbyLoading;
        Managers.LobbyManager.LobbyLoading += LobbyLoading;
    }


    protected override void StartInit()
    {
        base.StartInit();
        _loadingPanel.SetActive(false);
        SetSortingOrder(100);
    }

    public void LobbyLoading(bool isLobbyLoading)
    {
        if(_loadingPanel != null)
        _loadingPanel.SetActive(isLobbyLoading);
    }

}
