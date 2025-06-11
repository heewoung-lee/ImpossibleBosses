using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using GameManagers;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Scene : UI_Base
{
    protected override void AwakeInit()
    {
        Managers.UIManager.SetCanvas(gameObject.GetComponent<Canvas>(), true);
    }
    protected override void StartInit()
    {

    }
}
