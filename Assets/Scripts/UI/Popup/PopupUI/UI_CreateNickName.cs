using GameManagers;
using Module.UI_Module;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_CreateNickName : UI_Popup
{
    enum InputFields
    {
        NickNameInputField
    }
    enum Buttons
    {
        Confirm_Button
    }
    enum GameObjects
    {
        MessageError
    }
    private TMP_InputField _nickNameInputField;
    private Button _confirm_Button;
    private GameObject _messageError;
    private TMP_Text _errorMessageText;
    private ModuleUIFadeOut _errorMessageTextFadeOutMoudule;
    public PlayerLoginInfo PlayerLoginInfo { get; set; }

    protected override void OnDisableInit()
    {
        base.OnDisableInit();
        _nickNameInputField.text = "";
    }
    protected override void StartInit()
    {
        
    }

    protected override void AwakeInit()
    {
        base.AwakeInit();
        Bind<TMP_InputField>(typeof(InputFields));
        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));
        _nickNameInputField = Get<TMP_InputField>((int)InputFields.NickNameInputField);
        _confirm_Button = Get<Button>((int)Buttons.Confirm_Button);
        _messageError = Get<GameObject>((int)GameObjects.MessageError);
        _errorMessageText = _messageError.GetComponentInChildren<TMP_Text>();
        _errorMessageTextFadeOutMoudule = _messageError.GetComponent<ModuleUIFadeOut>();
        _errorMessageTextFadeOutMoudule.DoneFadeoutEvent += () => _confirm_Button.interactable = true;
        _messageError.SetActive(false);
        _confirm_Button.onClick.AddListener(CreateNickname);
    }

    public void CreateNickname()
    {
        _confirm_Button.interactable = false;
        CreateUserNickName(PlayerLoginInfo, _nickNameInputField.text);
    }

    public async void CreateUserNickName(PlayerLoginInfo playerinfo, string Nickname)
    {
        (bool isCheckResult, string message) = await Managers.LogInManager.WriteNickNameToGoogleSheet(playerinfo, Nickname);

        if (isCheckResult == false)
        {
            _messageError.SetActive(true);
            _errorMessageText.text = message;
        }
        else
        {
            Managers.UIManager.ClosePopupUI(this);
        }
    }
}
