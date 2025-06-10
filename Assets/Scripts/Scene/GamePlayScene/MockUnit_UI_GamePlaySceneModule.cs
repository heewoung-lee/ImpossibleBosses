using UnityEngine;

public class MockUnit_UI_GamePlaySceneModule : AddInGameUIModule
{
    protected override void StartInit()
    {
        base.StartInit();
        gameObject.AddComponent<Module_UI_Player_TestButton>();
    }
}
