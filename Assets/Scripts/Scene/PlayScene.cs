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
    }

    public override void Clear()
    {

    }

}