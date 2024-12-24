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
    private IMoveableState _currentState;

    private void Update()
    {
        _currentState = CurrentStateType as IMoveableState;
        if (_currentState != null)
        {
            Debug.Log(_currentState.ToString());
            _currentState.UpdateState(this);
        }
    }

    public abstract void UpdateAttack();
    public abstract void UpdateIdle();
    public abstract void UpdateMove();
    public abstract void UpdateDie();
}
