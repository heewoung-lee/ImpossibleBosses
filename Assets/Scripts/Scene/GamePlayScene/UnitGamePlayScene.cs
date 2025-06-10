using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class UnitGamePlayScene : ISceneSpawnBehaviour
{
    private UI_Stage_Timer _ui_stage_timer;
    private UI_Loading _ui_Loading_Scene;
    private GamePlaySceneLoadingProgress _gamePlaySceneLoadingProgress;

    public ISceneMover nextscene => new BattleSceneMover();

    public void Init()
    {
        _ui_Loading_Scene = Managers.UI_Manager.GetOrCreateSceneUI<UI_Loading>();
        _ui_stage_timer = Managers.UI_Manager.GetOrCreateSceneUI<UI_Stage_Timer>();
        _ui_stage_timer.OnTimerCompleted += nextscene.MoveScene;
    }
    public void SpawnOBJ()
    {
        if (Managers.RelayManager.NetworkManagerEx.IsHost)
        {
            Managers.RelayManager.Load_NGO_Prefab<NGO_GamePlaySceneSpawn>();
        }
    }
   
}
