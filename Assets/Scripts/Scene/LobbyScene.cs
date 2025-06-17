using GameManagers;
using Scene.GamePlayScene;
using Util;

namespace Scene
{
    public class LobbyScene : BaseScene
    {
        public override Define.Scene CurrentScene => Define.Scene.LobbyScene;
        public override ISceneSpawnBehaviour SceneSpawnBehaviour { get; }
        UI_LobbyScene _uiLobbyScene;
        public override void Clear()
        {
        }

        protected override void AwakeInit()
        {
        }

        protected override void StartInit()
        {
            base.StartInit();
            _uiLobbyScene = Managers.UIManager.GetSceneUIFromResource<UI_LobbyScene>();

        }
    }
}
