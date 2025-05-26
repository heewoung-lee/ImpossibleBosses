using UnityEngine;

public class MoveSceneController
{
    private ISceneSpawnBehaviour _iSceneBehaviour;

    public MoveSceneController(ISceneSpawnBehaviour iSceneBehaviour)
    {
        this._iSceneBehaviour = iSceneBehaviour;
    }


    public void InitGamePlayScene()
    {
        _iSceneBehaviour.Init();
    }


    public void SpawnOBJ()
    {
        _iSceneBehaviour.SpawnOBJ();
    }
}
