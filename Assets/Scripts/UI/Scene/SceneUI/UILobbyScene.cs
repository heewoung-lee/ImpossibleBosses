using GameManagers;
using Zenject;

namespace UI.Scene.SceneUI
{
    public class UILobbyScene : UIScene
    {
        [Inject]private UIManager _uiManager; 
        UIUserInfoPanel _uiUserPanel;
        UILobbyChat _uiLobbyChat;
        UIRoomInventory _uiRoomInventory;
        UILoadingPanel _uiLoadingPanel;
        protected override void AwakeInit()
        {
            base.AwakeInit();
            _uiUserPanel = _uiManager.GetSceneUIFromResource<UIUserInfoPanel>();
            _uiLobbyChat = _uiManager.GetSceneUIFromResource<UILobbyChat>();
            _uiRoomInventory = _uiManager.GetSceneUIFromResource<UIRoomInventory>();
            _uiLoadingPanel = _uiManager.GetSceneUIFromResource<UILoadingPanel>();
        }


        protected override void StartInit()
        {
            base.StartInit();
        }


    }
}
