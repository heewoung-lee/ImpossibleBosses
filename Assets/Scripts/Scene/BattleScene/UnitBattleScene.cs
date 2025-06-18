using GameManagers;
using Module.UI_Module;
using NetWork.NGO.UI;
using Scene.GamePlayScene;
using UnityEditor.SceneManagement;
using Zenject;

namespace Scene.BattleScene
{
    public class UnitBattleScene : ISceneSpawnBehaviour
    {
        private GamePlaySceneLoadingProgress _gamePlaySceneLoadingProgress;
        private UI_Loading _uiLoadingScene;
        private InGameUIModule _inGameUIModule;
        [Inject] private UIManager _uiManager;


        public ISceneMover Nextscene => new GamePlaySceneMover();


        public void Init()
        {
            _uiLoadingScene = _uiManager.GetOrCreateSceneUI<UI_Loading>();
        }

        public void SpawnObj()
        {
            if (Managers.RelayManager.NetworkManagerEx.IsHost)
            {
                Managers.RelayManager.Load_NGO_Prefab<NgoBattleSceneSpawn>();
            }
        }
    }
}
