using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Room_Info_Panel : UI_Base
{

    enum Texts
    {
        RoomNameText,
        CurrentCount
    }
    enum Buttons { JoinButton }



    private TMP_Text _roomNameText;
    private TMP_Text _currentPlayerCount;
    private Button _joinButton;
    protected override void AwakeInit()
    {
        Bind<TMP_Text>(typeof(Texts));
        Bind<Button>(typeof(Buttons));
        _roomNameText = Get<TMP_Text>((int)Texts.RoomNameText);
        _currentPlayerCount = Get<TMP_Text>((int)Texts.CurrentCount);
        _joinButton = Get<Button>((int)Buttons.JoinButton);
    }

    protected override void StartInit()
    {
    }
    
}
