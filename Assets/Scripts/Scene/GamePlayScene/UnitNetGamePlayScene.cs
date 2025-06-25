using GameManagers;
using GameManagers.Interface.UI_Interface;
using GameManagers.Interface.UIManager;
using NetWork.NGO.UI;
using Scene.BattleScene;
using Zenject;

namespace Scene.GamePlayScene
{
    public class UnitNetGamePlayScene : ISceneSpawnBehaviour
    {
        [Inject] private IUISceneManager _uiSceneManager;
        private UIStageTimer _uiStageTimer;
        private UI_Loading _uiLoadingScene;
        private GamePlaySceneLoadingProgress _gamePlaySceneLoadingProgress;

        public ISceneMover Nextscene => new BattleSceneMover();

        public void Init()
        {
            _uiLoadingScene = _uiSceneManager.GetOrCreateSceneUI<UI_Loading>();
            _uiStageTimer = _uiSceneManager.GetOrCreateSceneUI<UIStageTimer>();
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
