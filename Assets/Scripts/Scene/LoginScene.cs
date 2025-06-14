using GameManagers;
using UI.Scene.SceneUI;
using Util;

namespace Scene
{
    public class LoginScene : BaseScene
    {
        UI_LoginTitle _uiLoginTitle;

        public override Define.Scene CurrentScene => Define.Scene.LoginScene;

        protected override void StartInit()
        {
            base.StartInit();
            _uiLoginTitle = Managers.UIManager.GetSceneUIFromResource<UI_LoginTitle>();
        }

        protected override void AwakeInit()
        {
        }
        public override void Clear()
        {

        }
    }
}
