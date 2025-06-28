using GameManagers;
using GameManagers.Interface.UIManager;
using Module.UI_Module;
using NetWork.NGO.UI;
using Scene.GamePlayScene;
using UnityEditor.SceneManagement;
using Zenject;

namespace Scene.BattleScene
{
    public class UnitBattleScene : ISceneSpawnBehaviour
    {
        [Inject] private IUISceneManager _uiSceneManager;
        [Inject] private RelayManager _relayManager;
        
        private GamePlaySceneLoadingProgress _gamePlaySceneLoadingProgress;
        private UI_Loading _uiLoadingScene;
        private InGameUIModule _inGameUIModule;
        


        public ISceneMover Nextscene => new GamePlaySceneMover();


        public void Init()
        {
            _uiLoadingScene = _uiSceneManager.GetOrCreateSceneUI<UI_Loading>();
        }

        public void SpawnObj()
        {
            if (_relayManager.NetworkManagerEx.IsHost)
            {
                _relayManager.Load_NGO_Prefab<NgoBattleSceneSpawn>();
            }
        }
    }
}
