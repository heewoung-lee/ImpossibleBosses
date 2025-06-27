using GameManagers;
using GameManagers.Interface.UIManager;
using Zenject;

namespace UI.Scene.SceneUI
{
    public class UILobbyScene : UIScene
    {
        [Inject]private IUISceneManager _uiSceneManager; 
        UIUserInfoPanel _uiUserPanel;
        UILobbyChat _uiLobbyChat;
        UIRoomInventory _uiRoomInventory;
        UILoadingPanel _uiLoadingPanel;
        protected override void AwakeInit()
        {
            base.AwakeInit();
            _uiUserPanel = _uiSceneManager.GetSceneUIFromResource<UIUserInfoPanel>();
            _uiLobbyChat = _uiSceneManager.GetSceneUIFromResource<UILobbyChat>();
            _uiRoomInventory = _uiSceneManager.GetSceneUIFromResource<UIRoomInventory>();
            _uiLoadingPanel = _uiSceneManager.GetSceneUIFromResource<UILoadingPanel>();
        }


        protected override void StartInit()
        {
            base.StartInit();
        }


    }
}
