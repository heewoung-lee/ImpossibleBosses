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
        LogInTestToggle testTogle = Managers.UI_Manager.MakeSubItem<LogInTestToggle>( path: "Prefabs/UI/TestUI/LogInTestToggle");
        testTogle.transform.SetParent(ui_login.transform);
    }

    protected override void AwakeInit()
    {
        //여기에 
    }
    public override void Clear()
    {

    }
}
