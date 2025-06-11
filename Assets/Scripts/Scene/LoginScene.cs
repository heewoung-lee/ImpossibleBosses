using GameManagers;
using UnityEngine;

public class LoginScene : BaseScene
{
    UI_LoginTitle ui_login;

    public override Define.Scene CurrentScene => Define.Scene.LoginScene;

    protected override void StartInit()
    {
        base.StartInit();
        ui_login = Managers.UIManager.GetSceneUIFromResource<UI_LoginTitle>();
        
    }

    protected override void AwakeInit()
    {
    }
    public override void Clear()
    {

    }
}
