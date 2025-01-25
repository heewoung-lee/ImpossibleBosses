using UnityEngine;

public class LoginScene : BaseScene
{
    UI_LoginTitle ui_login;

    public override Define.Scene CurrentScene => Define.Scene.LoginScene;

    protected override void StartInit()
    {
        base.StartInit();
        ui_login = Managers.UI_Manager.GetSceneUIFromResource<UI_LoginTitle>();
        //테스트용 UI
        LogInTestToggle testTogle = Managers.UI_Manager.GetSceneUIFromResource<LogInTestToggle>(path: "Prefabs/UI/TestUI/LogInTestToggle");
    }

    protected override void AwakeInit()
    {
    }
    public override void Clear()
    {

    }
}
