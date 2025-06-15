using Module.UI_Module;
using Scene;
using Scene.GamePlayScene;

public class UnitLocalGamePlayScene : ISceneSpawnBehaviour
{
    public ISceneMover Nextscene { get; }
    public InGameUIModule InGameUIModule { get; }

    public void SpawnObj()
    {
        throw new System.NotImplementedException();
    }

    public void Init()
    {
        throw new System.NotImplementedException();
    }
}
