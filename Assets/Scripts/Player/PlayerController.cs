using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerController : MoveableController
{
    enum PlayerState
    {
        Pickup
    }


    InputManager _inputmanager;
    NavMeshAgent _agent;
    PlayerStats _stats;

    private PlayerInput _playerInput;
    private InputAction _moveAction;
    private InputAction _pointerAction;
    private InputAction _attackAction;
    private InputAction _stopAction;

    public Func<InputAction.CallbackContext, Vector3> ClickPositionEvent;
    public override Define.WorldObject WorldobjectType { get; protected set; } = Define.WorldObject.Player;

    protected override int Hash_Idle => Player_Anim_Hash.Idle;

    protected override int Hash_Move => Player_Anim_Hash.Run;

    protected override int Hash_Attack => Player_Anim_Hash.Attack;
    protected override int Hash_Die => Player_Anim_Hash.Die;

    protected override void AwakeInit()
    {
        _stats = GetComponent<PlayerStats>();
        _agent = GetComponent<NavMeshAgent>();
        _playerInput = GetComponent<PlayerInput>();

        _inputmanager = Managers.InputManager;
        _playerInput.actions = _inputmanager.InputActionAsset;
        _moveAction = _inputmanager.GetInputAction(Define.ControllerType.Player, "Move");
        _pointerAction = _inputmanager.GetInputAction(Define.ControllerType.Player, "Pointer");
        _attackAction = _inputmanager.GetInputAction(Define.ControllerType.Player, "Attack");
        _stopAction = _inputmanager.GetInputAction(Define.ControllerType.Player, "Stop");
    }

    private void OnEnable()
    {
        ClickPositionEvent += MouseRightClickPosEvent;
        _moveAction.performed += MouseRightClickEvent;
        _attackAction.performed += ComboAttack;
        _stopAction.performed += StopCommand;
    }
    private void OnDisable()
    {
        ClickPositionEvent -= MouseRightClickPosEvent;
        _moveAction.performed -= MouseRightClickEvent;
        _attackAction.performed -= ComboAttack;
        _stopAction.performed -= StopCommand;
    }
    protected override void StartInit()
    {
        _stats.PlayerDeadEvent -= PlayerDead;
        _stats.PlayerDeadEvent += PlayerDead;
    }
    private Vector3 MouseRightClickPosEvent(InputAction.CallbackContext context)
    {
        if (State == Define.State.Die)
            return Vector3.zero;

        Ray ray = Camera.main.ScreenPointToRay(_pointerAction.ReadValue<Vector2>());

        RaycastHit hit;
        Debug.DrawRay(Camera.main.transform.position, ray.direction * 100, Color.red);
        if (Physics.Raycast(ray, out hit, 100f, LayerMask.GetMask("Ground")))
        {
            _destPos = hit.point;
            if (State != Define.State.Move)
                State = Define.State.Move;
        }
        return hit.point;
    }
    private void MouseRightClickEvent(InputAction.CallbackContext context)
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        Vector3 clickPos = MouseRightClickPosEvent(context);
        _inputmanager.playerMouseClickPositionEvent?.Invoke(clickPos);
    }

    public void PlayerDead()
    {
        State = Define.State.Die;
    }

    private void ComboAttack(InputAction.CallbackContext context)
    {
        if (State == Define.State.Attack)
            return;

        State = Define.State.Attack;
    }
    private void StopCommand(InputAction.CallbackContext context)
    {
        State = Define.State.Idle;
    }

    protected override void UpdateMove()
    {
        if (State == Define.State.Die)
            return;
        Vector3 dir = new Vector3(_destPos.x, 0, _destPos.z) - new Vector3(transform.position.x, 0, transform.position.z);//높이에 대한 값을 빼야 근사값에 더 정확한 수치를 뽑을 수 있음.
        if (dir.magnitude < 0.01f)
        {
            State = Define.State.Idle;
        }
        else
        {
            Debug.DrawRay(transform.position + Vector3.up * 0.5f, dir, Color.green);
            if (Physics.Raycast(transform.position + Vector3.up * 0.5f, dir, 1.0f, LayerMask.GetMask("Block")))
            {
                State = Define.State.Idle;
                return;
            }
            float moveTick = Mathf.Clamp(_stats.MoveSpeed * Time.deltaTime, 0, dir.magnitude);
            _agent.Move(dir.normalized * moveTick);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 10 * Time.deltaTime);
        }
    }
    protected override void UpdateDie()
    {
    }
    protected override void UpdateIdle()
    {

    }
    protected override void UpdateAttack()
    {
        ChangeStateToIdle();
    }
    private void ChangeStateToIdle()
    {
        AnimatorStateInfo stateInfo = Anim.GetCurrentAnimatorStateInfo(AnimLayer);
        // Attack 스테이트가 정상 재생 중이며, 재생이 끝났는지 검사
        if (Anim.IsInTransition(AnimLayer) == false && stateInfo.IsName("Attack") && stateInfo.normalizedTime >= 1.0f)
        {
            State = Define.State.Idle;
        }
    }

    #region AnimationClipMethod
    public void AttackEvent()
    {
        TargetInSight.AttackTargetInSector(_stats);
    }
    #endregion
}
