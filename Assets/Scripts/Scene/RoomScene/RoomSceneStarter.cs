using GameManagers.Interface.UIManager;
using Scene.CommonInstaller;
using UI.Scene.SceneUI;
using Zenject;

namespace Scene.RoomScene
{
    public class RoomSceneStarter : ISceneStarter
    {
        [Inject] IUISceneManager _uiSceneManager;
        public void SceneStart()
        {
            UIRoomCharacterSelect uICharacterSelect = _uiSceneManager.GetSceneUIFromResource<UIRoomCharacterSelect>();
            UIRoomChat uiChatting = _uiSceneManager.GetSceneUIFromResource<UIRoomChat>();
        }
    }
}
