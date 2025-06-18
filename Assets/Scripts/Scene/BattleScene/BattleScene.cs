using GameManagers;
using Scene.GamePlayScene;
using Unity.VisualScripting;
using Util;
using Zenject;

namespace Scene.BattleScene
{
    public class BattleScene : BaseScene, ISkillInit
    {
        private UI_Loading _uiLoadingScene;
        private GamePlaySceneLoadingProgress _gamePlaySceneLoadingProgress;
        private ISceneSpawnBehaviour _sceneSpawnBehaviour;
        [Inject] private UIManager _uiManager;

        
        
        
        public bool isTest = false;
        public bool isSoloTest = false;
        public override Define.Scene CurrentScene => Define.Scene.BattleScene;
        public override ISceneSpawnBehaviour SceneSpawnBehaviour => _sceneSpawnBehaviour;


        protected override void StartInit()
        {
            base.StartInit();
            _uiLoadingScene = _uiManager.GetOrCreateSceneUI<UI_Loading>();
            _gamePlaySceneLoadingProgress = _uiLoadingScene.AddComponent<GamePlaySceneLoadingProgress>();
            // if (isTest == true)
            // {
            //     _battleSceneController = new MoveSceneController(new MockUnitBattleScene(Define.PlayerClass.Fighter, _uiLoadingScene, isSoloTest));
            //     gameObject.AddComponent<MockUnitUIGamePlaySceneModule>();
            //     _battleSceneController.InitGamePlayScene();
            // }
            // else
            // {
            //     _battleSceneController = new MoveSceneController(new UnitBattleScene());
            //     gameObject.AddComponent<MockUnitUIGamePlaySceneModule>();
            //     _battleSceneController.InitGamePlayScene();
            //     _gamePlaySceneLoadingProgress.OnLoadingComplete += _battleSceneController.SpawnObj;
            // }
            //
            
            _sceneSpawnBehaviour.Init();
            _sceneSpawnBehaviour.SpawnObj();
            //TODO: 0617 인르톨러가 없으니깐 반드시 인스톨러 넣을 것
        }
        public override void Clear()
        {
        }
        protected override void AwakeInit()
        {
        }
    }
}
