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
        //if (TryGetComponent(out GolemController controller))
        //{
        //   Debug.Log("현재 상태: " + State);
        //}

        switch (_state)
        {
            case Define.State.Die:
                UpdateDie();
                break;
            case Define.State.Move:
                UpdateMove();
                break;
            case Define.State.Idle:
                UpdateIdle();
                break;
            case Define.State.Attack:
                UpdateAttack();
                break;
        }
    }

    protected abstract void UpdateAttack();
    protected abstract void UpdateIdle();
    protected abstract void UpdateMove();
    protected abstract void UpdateDie();
}
