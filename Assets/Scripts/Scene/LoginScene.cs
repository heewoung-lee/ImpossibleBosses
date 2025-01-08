using UnityEngine;

public class LoginScene : BaseScene
{
    UI_LoginTitle ui_login;
    protected override void StartInit()
    {
        base.StartInit();
        currentScene = Define.Scene.Login;
        ui_login = Managers.UI_Manager.ShowSceneUI<UI_LoginTitle>();
    }

    protected override void AwakeInit()
    {
        //¿©±â¿¡ 
    }
    public override void Clear()
    {

    }
}
