using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Module_UI_PlayerInfo : MonoBehaviour
{
    void Start()
    {
        UI_Player_Info player_Info_UI = Managers.UI_Manager.GetSceneUIFromResource<UI_Player_Info>();
    }
}
