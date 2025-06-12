using GameManagers;
using NetWork.NGO.UI;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class UnitGamePlayScene : ISceneSpawnBehaviour
{
    private UIStageTimer _ui_stage_timer;
    private UI_Loading _ui_Loading_Scene;
    private GamePlaySceneLoadingProgress _gamePlaySceneLoadingProgress;

    public ISceneMover nextscene => new BattleSceneMover();

    public void Init()
    {
        _ui_Loading_Scene = Managers.UIManager.GetOrCreateSceneUI<UI_Loading>();
        _ui_stage_timer = Managers.UIManager.GetOrCreateSceneUI<UIStageTimer>();
        _ui_stage_timer.OnTimerCompleted += nextscene.MoveScene;
    }
    public void SpawnOBJ()
    {
        if (Managers.RelayManager.NetworkManagerEx.IsHost)
        {
            Managers.RelayManager.Load_NGO_Prefab<NgoGamePlaySceneSpawn>();
        }
    }
   
}
