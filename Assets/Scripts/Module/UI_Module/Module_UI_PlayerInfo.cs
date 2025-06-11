using System.Collections;
using System.Collections.Generic;
using GameManagers;
using UnityEngine;

public class Module_UI_PlayerInfo : MonoBehaviour
{
    void Start()
    {
        UI_Player_Info player_Info_UI = Managers.UIManager.GetSceneUIFromResource<UI_Player_Info>();
    }
}
