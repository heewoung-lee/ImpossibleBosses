using GameManagers;
using NetWork.NGO.UI;

namespace Scene.BattleScene
{
    public class UnitBattleScene : ISceneSpawnBehaviour
    {
        private UI_Loading _uiLoadingScene;
        private GamePlaySceneLoadingProgress _gamePlaySceneLoadingProgress;

        public ISceneMover nextscene => new GamePlaySceneMover();

        public void Init()
        {
            _uiLoadingScene = Managers.UIManager.GetOrCreateSceneUI<UI_Loading>();
        }

        public void SpawnOBJ()
        {
            if (Managers.RelayManager.NetworkManagerEx.IsHost)
            {
                Managers.RelayManager.Load_NGO_Prefab<NgoBattleSceneSpawn>();
            }
        }
    }
}
