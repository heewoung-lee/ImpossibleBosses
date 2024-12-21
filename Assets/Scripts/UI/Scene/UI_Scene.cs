using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Scene : UI_Base
{
    protected override void AwakeInit()
    {
        Managers.UI_Manager.SetCanvas(gameObject, true);
    }
    protected override void StartInit()
    {

    }
}
