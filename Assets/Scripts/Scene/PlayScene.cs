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

    protected override void AwakeInit()
    {
    }
    protected override void StartInit()
    {
        base.StartInit();
        if(Managers.RelayManager.NetWorkManager.IsHost)
        Managers.RelayManager.Load_NGO_ROOT_UI_Module("NGO/PlayerSpawner");
    }

    public override void Clear()
    {

    }

}