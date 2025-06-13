using GameManagers;
using NetWork.NGO.UI;
using Scene.GamePlayScene;

namespace Scene.BattleScene
{
    public class UnitBattleScene : ISceneSpawnBehaviour
    {
        private UI_Loading _uiLoadingScene;
        private GamePlaySceneLoadingProgress _gamePlaySceneLoadingProgress;

        public ISceneMover Nextscene => new GamePlaySceneMover();

        public void Init()
        {
            _uiLoadingScene = Managers.UIManager.GetOrCreateSceneUI<UI_Loading>();
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
