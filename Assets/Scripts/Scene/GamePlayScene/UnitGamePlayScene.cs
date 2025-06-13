using GameManagers;
using NetWork.NGO.UI;
using Scene.BattleScene;

namespace Scene.GamePlayScene
{
    public class UnitGamePlayScene : ISceneSpawnBehaviour
    {
        private UIStageTimer _uiStageTimer;
        private UI_Loading _uiLoadingScene;
        private GamePlaySceneLoadingProgress _gamePlaySceneLoadingProgress;

        public ISceneMover Nextscene => new BattleSceneMover();

        public void Init()
        {
            _uiLoadingScene = Managers.UIManager.GetOrCreateSceneUI<UI_Loading>();
            _uiStageTimer = Managers.UIManager.GetOrCreateSceneUI<UIStageTimer>();
            _uiStageTimer.OnTimerCompleted += Nextscene.MoveScene;
        }
    
    
    
    
        public void SpawnObj()
        {
            if (Managers.RelayManager.NetworkManagerEx.IsHost)
            {
                Managers.RelayManager.Load_NGO_Prefab<NgoGamePlaySceneSpawn>();
            }
        }
   
    }
}
