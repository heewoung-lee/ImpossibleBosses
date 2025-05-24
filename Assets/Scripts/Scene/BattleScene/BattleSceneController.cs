using UnityEngine;

public class BattleSceneController
{
    IBattleSceneSpawnBehaviour _battleSceneBahaviour;

    public BattleSceneController(IBattleSceneSpawnBehaviour ibattleSceneSpawnBahaviour)
    {
        _battleSceneBahaviour = ibattleSceneSpawnBahaviour;
    }
    public void Init()
    {
        _battleSceneBahaviour.Init();
    }
    public void SpawnOBJ()
    {
        _battleSceneBahaviour.SpawnOBJ();
    }

}
