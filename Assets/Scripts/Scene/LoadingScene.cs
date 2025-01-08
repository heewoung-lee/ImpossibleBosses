using UnityEngine;

public class LoadingScene : BaseScene
{
    UI_Loading _ui_loding;
    public override void Clear()
    {
        throw new System.NotImplementedException();
    }

    protected override void AwakeInit()
    {
        _ui_loding = Managers.UI_Manager.ShowSceneUI<UI_Loading>();
    }

}
