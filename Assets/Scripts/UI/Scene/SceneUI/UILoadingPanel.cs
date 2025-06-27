using GameManagers;
using UnityEngine;
using UnityEngine.UI;
using Util;
using Zenject;

namespace UI.Scene.SceneUI
{
    public class UILoadingPanel : UIScene
    {
        [Inject] private LobbyManager _lobbyManager;
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
            _lobbyManager.LobbyLoadingEvent += LobbyLoading;
        }


        protected override void StartInit()
        {
            base.StartInit();
            _loadingPanelImage.enabled = false;
            SetSortingOrder((int)Define.SpecialSortingOrder.LoadingPanel);
        }

        public void LobbyLoading(bool isLobbyLoading)
        {
            if(_loadingPanel != null)
            {
                _loadingPanelImage.ImageEnable(isLobbyLoading);
            }
        }
    }
}
