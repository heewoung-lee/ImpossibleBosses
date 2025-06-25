using GameManagers;
using GameManagers.Interface;
using GameManagers.Interface.UI_Interface;
using GameManagers.Interface.UIManager;
using Scene.GamePlayScene;
using UI.Scene.SceneUI;
using Util;
using Zenject;

namespace Scene
{
    public class LoginScene : BaseScene
    {
        [Inject]private IUISceneManager _sceneuiManager;
        private UILoginTitle _uiLoginTitle;

        
        public override Define.Scene CurrentScene => Define.Scene.LoginScene;
        public override ISceneSpawnBehaviour SceneSpawnBehaviour { get; }

        protected override void StartInit()
        {
            base.StartInit();
            _uiLoginTitle = _sceneuiManager.GetSceneUIFromResource<UILoginTitle>();
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
