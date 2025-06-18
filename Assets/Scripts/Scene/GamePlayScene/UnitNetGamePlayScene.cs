using GameManagers;
using NetWork.NGO.UI;
using Scene.BattleScene;
using Zenject;

namespace Scene.GamePlayScene
{
    public class UnitNetGamePlayScene : ISceneSpawnBehaviour
    {
        [Inject] private UIManager _uiManager;
        private UIStageTimer _uiStageTimer;
        private UI_Loading _uiLoadingScene;
        private GamePlaySceneLoadingProgress _gamePlaySceneLoadingProgress;

        public ISceneMover Nextscene => new BattleSceneMover();

        public void Init()
        {
            _uiLoadingScene = _uiManager.GetOrCreateSceneUI<UI_Loading>();
            _uiStageTimer = _uiManager.GetOrCreateSceneUI<UIStageTimer>();
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
