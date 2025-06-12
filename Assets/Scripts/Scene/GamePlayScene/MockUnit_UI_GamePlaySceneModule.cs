using Module.UI_Module;
using UnityEngine;

public class MockUnit_UI_GamePlaySceneModule : AddInGameUIModule
{
    protected override void StartInit()
    {
        base.StartInit();
        gameObject.AddComponent<ModuleUIPlayerTestButton>();
    }
}
