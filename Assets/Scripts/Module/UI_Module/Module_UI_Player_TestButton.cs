using System.Collections;
using System.Collections.Generic;
using GameManagers;
using UnityEngine;

public class Module_UI_Player_TestButton : MonoBehaviour
{

    void Start()
    {
        UI_CREATE_ITEM_AND_GOLD_Button button_UI = Managers.UIManager.GetSceneUIFromResource<UI_CREATE_ITEM_AND_GOLD_Button>();
    }

}
