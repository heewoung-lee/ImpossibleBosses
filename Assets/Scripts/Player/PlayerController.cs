using BaseStates;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerController : MoveableController
{
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
        if (CurrentStateType == typeof(DieState))
            return Vector3.zero;

        Ray ray = Camera.main.ScreenPointToRay(_pointerAction.ReadValue<Vector2>());

        RaycastHit hit;
        Debug.DrawRay(Camera.main.transform.position, ray.direction * 100, Color.red);
        if (Physics.Raycast(ray, out hit, 100f, LayerMask.GetMask("Ground")))
        {
            _destPos = hit.point;
            if (CurrentStateType != typeof(MoveState))
                CurrentStateType = typeof(MoveState);
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
        CurrentStateType = typeof(DieState);
    }

    private void ComboAttack(InputAction.CallbackContext context)
    {
        if (CurrentStateType == typeof(AttackState))
            return;

        CurrentStateType = typeof(AttackState);
    }
    private void StopCommand(InputAction.CallbackContext context)
    {
        CurrentStateType = typeof(IDleState);
    }

    public override void UpdateMove()
    {
        if (CurrentStateType == typeof(DieState))
            return;
        Vector3 dir = new Vector3(_destPos.x, 0, _destPos.z) - new Vector3(transform.position.x, 0, transform.position.z);//���̿� ���� ���� ���� �ٻ簪�� �� ��Ȯ�� ��ġ�� ���� �� ����.
        if (dir.magnitude < 0.01f)
        {
            CurrentStateType = typeof(IDleState);
        }
        else
        {
            Debug.DrawRay(transform.position + Vector3.up * 0.5f, dir, Color.green);
            if (Physics.Raycast(transform.position + Vector3.up * 0.5f, dir, 1.0f, LayerMask.GetMask("Block")))
            {
                CurrentStateType = typeof(IDleState);
                return;
            }
            float moveTick = Mathf.Clamp(_stats.MoveSpeed * Time.deltaTime, 0, dir.magnitude);
            _agent.Move(dir.normalized * moveTick);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 10 * Time.deltaTime);
        }
    }
    public override void UpdateDie()
    {
    }
    public override void UpdateIdle()
    {

    }
    public override void UpdateAttack()
    {
        ChangeStateToIdle();
    }
    private void ChangeStateToIdle()
    {
        AnimatorStateInfo stateInfo = Anim.GetCurrentAnimatorStateInfo(AnimLayer);
        // Attack ������Ʈ�� ���� ��� ���̸�, ����� �������� �˻�
        if (Anim.IsInTransition(AnimLayer) == false && stateInfo.IsName("Attack") && stateInfo.normalizedTime >= 1.0f)
        {
            CurrentStateType = typeof(IDleState);
        }
    }

    #region AnimationClipMethod
    public void AttackEvent()
    {
        TargetInSight.AttackTargetInSector(_stats);
    }
    #endregion
}
