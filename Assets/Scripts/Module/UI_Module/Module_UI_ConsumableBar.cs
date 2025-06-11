using System.Collections;
using System.Collections.Generic;
using GameManagers;
using UnityEngine;

public class Module_UI_ConsumableBar : MonoBehaviour
{

    void Start()
    {
        UI_ConsumableBar ui_ConsumableBar = Managers.UI_Manager.GetSceneUIFromResource<UI_ConsumableBar>();
    }

}
