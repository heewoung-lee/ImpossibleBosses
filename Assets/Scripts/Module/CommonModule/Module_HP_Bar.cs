using System.Collections;
using System.Collections.Generic;
using GameManagers;
using UnityEngine;

public class Module_HP_Bar : MonoBehaviour
{
    void Start()
    {
        UI_HPBar player_Info_UI = Managers.UIManager.MakeUIWorldSpaceUI<UI_HPBar>();
        player_Info_UI.transform.SetParent(transform);
    }
}
