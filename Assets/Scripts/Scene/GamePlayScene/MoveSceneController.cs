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


        public void InitGamePlayScene()
        {
            _iSceneBehaviour.Init();
        }


        public void SpawnObj()
        {
            _iSceneBehaviour.SpawnObj();
        }
    }
}
