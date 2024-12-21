using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Module_UI_ConsumableBar : MonoBehaviour
{

    void Start()
    {
        UI_ConsumableBar ui_ConsumableBar = Managers.UI_Manager.ShowSceneUI<UI_ConsumableBar>();
    }

}
