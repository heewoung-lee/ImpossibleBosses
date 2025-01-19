using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Module_UI_Player_TestButton : MonoBehaviour
{

    void Start()
    {
        UI_Button button_UI = Managers.UI_Manager.GetSceneUIFromResource<UI_Button>();
    }

}
