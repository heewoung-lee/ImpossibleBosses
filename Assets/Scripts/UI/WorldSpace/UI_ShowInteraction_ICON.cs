using UnityEngine;
using UnityEngine.UI;

public class UI_ShowInteraction_ICON : UI_Base
{
    Image _icon;
    enum IConImage
    {
        Icon
    }
    protected override void AwakeInit()
    {
        Bind<Image>(typeof(IConImage));
        _icon = GetImage((int)IConImage.Icon);
    }
    protected override void StartInit()
    {
    }


    private void LateUpdate()
    {
        transform.rotation = Camera.main.transform.rotation;
    }

}