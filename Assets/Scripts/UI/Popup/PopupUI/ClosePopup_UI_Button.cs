using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClosePopup_UI_Button : MonoBehaviour
{
    private Button _windowClose_Button;
    
    private UI_Popup parentPopup;
    void Start()
    {
        parentPopup = transform.FindParantComponent<UI_Popup>();

        _windowClose_Button = _windowClose_Button = Utill.FindChild(gameObject, "Button_Close", true).GetComponent<Button>();
        _windowClose_Button.onClick.AddListener(() =>
        {
            Managers.UI_Manager.ClosePopupUI(parentPopup);
        });
    }

}
