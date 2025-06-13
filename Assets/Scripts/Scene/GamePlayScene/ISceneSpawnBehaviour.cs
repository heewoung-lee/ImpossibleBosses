namespace Scene.GamePlayScene
{
    public interface ISceneSpawnBehaviour
    {
        ISceneMover Nextscene { get; }

        public void SpawnObj();

        public void Init();

    }
}
