using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.AI;

public class PlayScene : BaseScene
{
    public override Define.Scene CurrentScene => Define.Scene.GamePlayScene;

    private UI_Stage_Timer _ui_stage_timer;
    public UI_Stage_Timer UI_Stage_Timer => _ui_stage_timer;

    protected override void AwakeInit()
    {
    }
    protected override void StartInit()
    {
        base.StartInit();
        _ui_stage_timer = Managers.UI_Manager.GetSceneUIFromResource<UI_Stage_Timer>();
    }

    public override void Clear()
    {
    }

}