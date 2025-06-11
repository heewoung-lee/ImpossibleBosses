using System;
using GameManagers;
using Unity.Multiplayer.Playmode;
using UnityEngine;
using UnityEngine.UI;

public class LogInTestToggle : UI_Scene
{

    enum Buttons
    {
        Test_Button
    }
    enum Toggles 
    {
        LogInTestToggle
    }

    enum Players
    {
        Player1,
        Player2,
        Player3,
        Player4,
        None
    }


    Button _testButton;
    Toggle _testToggle;
    protected override void AwakeInit()
    {
        base.AwakeInit();
        Bind<Button>(typeof(Buttons));
        Bind<Toggle>(typeof(Toggles));
        _testButton = Get<Button>((int)Buttons.Test_Button);
        _testToggle = Get<Toggle>((int)Toggles.LogInTestToggle);
        _testButton.interactable = _testToggle.interactable;
        _testToggle.onValueChanged.AddListener((ison) =>
        {
            _testButton.gameObject.SetActive(ison);
        });
        _testButton.onClick.AddListener(ClickLogin);
    }

    private void ClickLogin()
    {
        UI_LoginPopup loginPopup = Managers.UI_Manager.TryGetPopupDictAndShowPopup<UI_LoginPopup>();


        Players currentPlayer = Players.Player1;

        string[] tagValue = CurrentPlayer.ReadOnlyTags();
            if (tagValue.Length > 0 && Enum.TryParse(typeof(Players), tagValue[0], out var parsedEnum))
            {
                currentPlayer = (Players)parsedEnum;
                Debug.Log($"Current player: {currentPlayer}");
            }
            switch (currentPlayer)
            {
                case Players.Player1:
                    loginPopup.AuthenticateUser("hiwoong123", "123123");
                    break;
                case Players.Player2:
                    loginPopup.AuthenticateUser("hiwoong12", "123123");
                    break;
                case Players.Player3:
                    loginPopup.AuthenticateUser("hiwoo12", "123123");
                    break;
                case Players.Player4:
                    loginPopup.AuthenticateUser("hiwoong1234", "123123");
                    break;
                case Players.None:
                    break;
        }
    }

    protected override void StartInit()
    {
        base.AwakeInit();
    }

}
