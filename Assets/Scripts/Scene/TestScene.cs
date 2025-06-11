using System.Collections;
using System.Collections.Generic;
using GameManagers;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class TestScene : BaseScene
{
    private GameObject _player;

    public override Define.Scene CurrentScene => Define.Scene.Unknown;

    protected override void StartInit()
    {
        base.StartInit();
    }

    public override void Clear()
    {

    }

    protected override void AwakeInit()
    {
        _player = Managers.GameManagerEx.Spawn("Prefabs/Player/Fighter");
    }
}