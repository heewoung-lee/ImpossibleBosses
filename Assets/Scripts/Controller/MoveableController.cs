using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(BaseStats))]
public abstract class MoveableController : BaseController
{
    protected Vector3 _destPos;
    PlayerController _player;
    private void Update()
    {
      CurrentStateType.UpdateState();
    }
}
