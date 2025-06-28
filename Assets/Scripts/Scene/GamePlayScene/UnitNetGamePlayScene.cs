using GameManagers;
using GameManagers.Interface.UI_Interface;
using GameManagers.Interface.UIManager;
using NetWork.NGO.UI;
using Scene.BattleScene;
using UI.Scene.SceneUI;
using Zenject;

namespace Scene.GamePlayScene
{
    public class UnitNetGamePlayScene : ISceneSpawnBehaviour
    {
        [Inject] private IUISceneManager _uiSceneManager;
        [Inject] private RelayManager _relayManager;

        private UIStageTimer _uiStageTimer;
        private UILoading _uiLoadingScene;
        private GamePlaySceneLoadingProgress _gamePlaySceneLoadingProgress;

        public ISceneMover Nextscene => new BattleSceneMover();

        public void Init()
        {
            _uiLoadingScene = _uiSceneManager.GetOrCreateSceneUI<UILoading>();
            _uiStageTimer = _uiSceneManager.GetOrCreateSceneUI<UIStageTimer>();
            _uiStageTimer.OnTimerCompleted += Nextscene.MoveScene;
        }
    
    
    
    
        public void SpawnObj()
        {
            if (_relayManager.NetworkManagerEx.IsHost)
            {
                _relayManager.Load_NGO_Prefab<NgoGamePlaySceneSpawn>();
            }
        }
   
    }
}
