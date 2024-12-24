using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ShowInteraction_ICON : UI_Base
{
    private Image _interactionIcon;
    private TMP_Text _interactionName;

    enum IConImage
    {
        Icon
    }
    enum Texts
    {
        InteractionName
    }
    public void SetInteractionText(string name, Color color)
    {
        _interactionName.text = name;
        _interactionName.color = color;
    }
    protected override void AwakeInit()
    {
        Bind<Image>(typeof(IConImage));
        Bind<TMP_Text>(typeof(Texts));
        _interactionIcon = GetImage((int)IConImage.Icon);
        _interactionName = GetText((int)Texts.InteractionName);
    }
    protected override void StartInit()
    {
    }

    private void LateUpdate()
    {
        transform.rotation = Camera.main.transform.rotation;
    }

}