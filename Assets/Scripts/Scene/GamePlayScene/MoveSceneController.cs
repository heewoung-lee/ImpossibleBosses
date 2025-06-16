using Module.UI_Module;

namespace Scene.GamePlayScene
{
    public class MoveSceneController
    {
        private ISceneSpawnBehaviour _iSceneBehaviour;
        public ISceneSpawnBehaviour ISceneBehaviour => _iSceneBehaviour;
        
        public MoveSceneController(ISceneSpawnBehaviour iSceneBehaviour)
        {
            this._iSceneBehaviour = iSceneBehaviour;
        }
//컨트롤러가 어떤 걸 주입받냐에 따라 어떻게 행동할껀지 분기해줘야함.

        public void InitGamePlayScene()
        {
            _iSceneBehaviour.Init();
            _iSceneBehaviour.SpawnObj();
        }

        public void SpawnObj()
        {
            _iSceneBehaviour.SpawnObj();
        }

    }
}
