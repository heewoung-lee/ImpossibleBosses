using Module.UI_Module;

namespace Scene.GamePlayScene
{
    public class MockUnitUIGamePlaySceneModule : InGameUIModule
    {
        protected override void StartInit()
        {
            base.StartInit();
            gameObject.AddComponent<ModuleUIPlayerTestButton>();
        }
    }
}
