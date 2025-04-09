using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Module_PortalIndicator : MonoBehaviour
{
    void Start()
    {
        UI_PortalIndicator ui_ProtalIndicator = Managers.UI_Manager.MakeUIWorldSpaceUI<UI_PortalIndicator>();
        ui_ProtalIndicator.transform.SetParent(transform);
    }
}
