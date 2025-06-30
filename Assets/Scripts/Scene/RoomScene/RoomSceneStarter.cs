using GameManagers.Interface.UIManager;
using UI.Scene.SceneUI;
using Zenject;

namespace Scene.RoomScene
{
    public class RoomSceneStarter : IRoomSceneStarter
    {
        [Inject] IUISceneManager _uiSceneManager;
        public void RoomSceneStart()
        {
            UIRoomCharacterSelect uICharacterSelect = _uiSceneManager.GetSceneUIFromResource<UIRoomCharacterSelect>();
            UIRoomChat uiChatting = _uiSceneManager.GetSceneUIFromResource<UIRoomChat>();
        }
    }
}
