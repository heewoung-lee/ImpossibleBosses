using GameManagers;
using Scene.GamePlayScene;
using UI.Scene.SceneUI;
using Util;
using Zenject;

namespace Scene
{
    public class LobbyScene : BaseScene
    {
        public override Define.Scene CurrentScene => Define.Scene.LobbyScene;
        public override ISceneSpawnBehaviour SceneSpawnBehaviour { get; }
        
        [Inject]private UIManager _uiManager; 
        UILobbyScene _uiLobbyScene;
        public override void Clear()
        {
        }

        protected override void AwakeInit()
        {
        }

        protected override void StartInit()
        {
            base.StartInit();
            _uiLobbyScene = _uiManager.GetSceneUIFromResource<UILobbyScene>();

        }
    }
}
