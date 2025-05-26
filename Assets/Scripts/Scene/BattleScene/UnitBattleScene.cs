using UnityEngine;

public class UnitBattleScene : ISceneSpawnBehaviour
{
    private UI_Loading _ui_Loading_Scene;
    private GamePlaySceneLoadingProgress _gamePlaySceneLoadingProgress;

    public ISceneMover nextscene => new GamePlaySceneMover();

    public void Init()
    {
        _ui_Loading_Scene = Managers.UI_Manager.GetOrCreateSceneUI<UI_Loading>();
    }

    public void SpawnOBJ()
    {
        if (Managers.RelayManager.NetworkManagerEx.IsHost)
        {
            Managers.RelayManager.Load_NGO_Prefab<NGO_BattleSceneSpawn>();
        }
    }
}
