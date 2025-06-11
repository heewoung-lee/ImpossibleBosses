using System.Collections;
using System.Collections.Generic;
using GameManagers;
using UnityEngine;

public class Module_PortalIndicator : MonoBehaviour
{
    void Start()
    {
        UI_PortalIndicator ui_ProtalIndicator = Managers.UIManager.MakeUIWorldSpaceUI<UI_PortalIndicator>();
        ui_ProtalIndicator.transform.SetParent(transform);
    }
}
