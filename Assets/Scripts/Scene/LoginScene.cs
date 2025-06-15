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
            Managers.SceneManagerEx.SetBootMode(true);
            //로그인 상태부터 돌리는 씬은 노멀 루트이므로 테스트모드가 아니다.
        }

        protected override void AwakeInit()
        {
        }
        public override void Clear()
        {

        }
    }
}
