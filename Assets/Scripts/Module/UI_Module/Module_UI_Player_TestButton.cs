using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Module_UI_Player_TestButton : MonoBehaviour
{

    void Start()
    {
        UI_CREATE_ITEM_AND_GOLD_Button button_UI = Managers.UI_Manager.GetSceneUIFromResource<UI_CREATE_ITEM_AND_GOLD_Button>(isCheckDontDestroy: true);
    }

}
