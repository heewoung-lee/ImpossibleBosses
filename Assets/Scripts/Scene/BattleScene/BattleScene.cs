using GameManagers;
using Scene.GamePlayScene;
using Unity.VisualScripting;

namespace Scene.BattleScene
{
    public class BattleScene : BaseScene, ISkillInit, ISceneController
    {
        private UI_Loading _uiLoadingScene;
        private GamePlaySceneLoadingProgress _gamePlaySceneLoadingProgress;
        private MoveSceneController _battleSceneController;

        public bool isTest = false;
        public bool isSoloTest = false;
        public override Define.Scene CurrentScene => Define.Scene.BattleScene;
        public MoveSceneController SceneMoverController => _battleSceneController;

        protected override void StartInit()
        {
            base.StartInit();
            _uiLoadingScene = Managers.UIManager.GetOrCreateSceneUI<UI_Loading>();
            _gamePlaySceneLoadingProgress = _uiLoadingScene.AddComponent<GamePlaySceneLoadingProgress>();
            if (isTest == true)
            {
                _battleSceneController = new MoveSceneController(new MockUnitBattleScene(Define.PlayerClass.Fighter, _uiLoadingScene, isSoloTest));
                gameObject.AddComponent<MockUnitUIGamePlaySceneModule>();
                _battleSceneController.InitGamePlayScene();
                _battleSceneController.SpawnObj();
            }
            else
            {
                _battleSceneController = new MoveSceneController(new UnitBattleScene());
                gameObject.AddComponent<MockUnitUIGamePlaySceneModule>();
                _battleSceneController.InitGamePlayScene();
                _gamePlaySceneLoadingProgress.OnLoadingComplete += _battleSceneController.SpawnObj;
            }
        }
        public override void Clear()
        {
        }
        protected override void AwakeInit()
        {
        }
    }
}
