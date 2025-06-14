using GameManagers;
using Util;

namespace Scene
{
    public class LobbyScene : BaseScene
    {
        public override Define.Scene CurrentScene => Define.Scene.LobbyScene;
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
