using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(BaseStats))]
public abstract class MoveableController : BaseController
{
    protected Vector3 _destPos;
    protected GameObject _target;
    PlayerController _player;
    private void Update()
    {
      CurrentStateType.UpdateState();
    }

    public abstract void UpdateAttack();
    public abstract void UpdateIdle();
    public abstract void UpdateMove();
    public abstract void UpdateDie();
}
