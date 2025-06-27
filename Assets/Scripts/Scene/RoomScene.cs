using GameManagers;
using GameManagers.Interface.UIManager;
using Scene.GamePlayScene;
using UI.Scene.SceneUI;
using Util;
using Zenject;

namespace Scene
{
    public class RoomScene : BaseScene
    {
        [Inject]private IUISceneManager _uiSceneManager;
        public override Define.Scene CurrentScene => Define.Scene.RoomScene;
        public override ISceneSpawnBehaviour SceneSpawnBehaviour { get; }

        public override void Clear()
        {
        }

        protected override void AwakeInit()
        {
        }

        protected override void StartInit()
        {
            base.StartInit();
      
            UIRoomCharacterSelect uICharacterSelect = _uiSceneManager.GetSceneUIFromResource<UIRoomCharacterSelect>();
            UIRoomChat uiChatting = _uiSceneManager.GetSceneUIFromResource<UIRoomChat>();
        }
    }
}
