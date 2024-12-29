using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class TestScene : BaseScene
{
    private GameObject _player;
    protected override void StartInit()
    {
        base.StartInit();
    }

    public override void Clear()
    {

    }

    protected override void AwakeInit()
    {
        _player = Managers.GameManagerEx.Spawn("Prefabs/Player/Player");
    }
}