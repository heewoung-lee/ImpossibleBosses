using BaseStates;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerController : MoveableController
{
    private const float DEFALUT_TRANSITION_PICKUP = 0.3f;
    InputManager _inputmanager;
    NavMeshAgent _agent;
    PlayerStats _stats;

    private PlayerInput _playerInput;
    private InputAction _moveAction;
    private InputAction _pointerAction;
    private InputAction _attackAction;
    private InputAction _stopAction;
    private NetworkObject _playerNGO;

    public Func<InputAction.CallbackContext, Vector3> ClickPositionEvent;
    public override Define.WorldObject WorldobjectType { get; protected set; } = Define.WorldObject.Player;
    protected override int Hash_Idle => Player_Anim_Hash.Idle;
    protected override int Hash_Move => Player_Anim_Hash.Run;
    protected override int Hash_Attack => Player_Anim_Hash.Attack;
    protected override int Hash_Die => Player_Anim_Hash.Die;
    private int _hash_PickUp => Animator.StringToHash("Pickup");

    public override AttackState Base_Attackstate => _base_Attackstate;
    public override IDleState Base_IDleState => _base_IDleState;
    public override DieState Base_DieState => _base_DieState;
    public override MoveState Base_MoveState => _base_MoveState;

    public PickUpState PickupState => _pickup_State;

    private AttackState _base_Attackstate;
    private IDleState _base_IDleState;
    private DieState _base_DieState;
    private MoveState _base_MoveState;
    private PickUpState _pickup_State;

    protected override void AwakeInit()
    {
        _stats = gameObject.GetComponent<PlayerStats>();
        _agent = gameObject.GetComponent<NavMeshAgent>();
        _playerInput = gameObject.GetComponent<PlayerInput>();
        _playerNGO = GetComponent<NetworkObject>();

        _inputmanager = Managers.InputManager;
        _playerInput.actions = _inputmanager.InputActionAsset;
        _moveAction = _inputmanager.GetInputAction(Define.ControllerType.Player, "Move");
        _pointerAction = _inputmanager.GetInputAction(Define.ControllerType.Player, "Pointer");
        _attackAction = _inputmanager.GetInputAction(Define.ControllerType.Player, "Attack");
        _stopAction = _inputmanager.GetInputAction(Define.ControllerType.Player, "Stop");
        _moveAction.Enable();
        _pointerAction.Enable(); 
        _attackAction.Enable();
        _stopAction.Enable();

        _base_Attackstate = new AttackState(UpdateAttack);
        _base_MoveState = new MoveState(UpdateMove);
        _base_DieState = new DieState(UpdateDie);
        _base_IDleState = new IDleState(UpdateIdle);
        _pickup_State = new PickUpState(UpdatePickup);
    }

    private void OnEnable()
    {
        ClickPositionEvent += MouseRightClickPosEvent;
        _moveAction.performed += MouseRightClickEvent;
        _attackAction.performed += Attack;
        _stopAction.performed += StopCommand;
    }
    private void OnDisable()
    {
        ClickPositionEvent -= MouseRightClickPosEvent;
        _moveAction.performed -= MouseRightClickEvent;
        _attackAction.performed -= Attack;
        _stopAction.performed -= StopCommand;
    }
    protected override void StartInit()
    {
        _stats.PlayerDeadEvent -= PlayerDead;
        _stats.PlayerDeadEvent += PlayerDead;
    }
    private Vector3 MouseRightClickPosEvent(InputAction.CallbackContext context)
    {
        Ray ray = Camera.main.ScreenPointToRay(_pointerAction.ReadValue<Vector2>());
        RaycastHit hit;

        Debug.DrawRay(Camera.main.transform.position, ray.direction * 100, Color.red);
        if (Physics.Raycast(ray, out hit, 100f, LayerMask.GetMask("Ground")))
        {
            _destPos = hit.point;
            if (CurrentStateType != _base_MoveState)
                CurrentStateType = _base_MoveState;
        }
        return hit.point;
    }
    private void MouseRightClickEvent(InputAction.CallbackContext context)
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;


        Vector2 screenPos = _pointerAction.ReadValue<Vector2>();
        if (screenPos.x < 0f || screenPos.x > Screen.width ||
            screenPos.y < 0f || screenPos.y > Screen.height)
            return;

        Vector3 clickPos = MouseRightClickPosEvent(context);
        _inputmanager.playerMouseClickPositionEvent?.Invoke(clickPos);
    }

    public void PlayerDead()
    {
        CurrentStateType = Base_DieState;
    }

    private void Attack(InputAction.CallbackContext context)
    {
        if (CurrentStateType == Base_Attackstate)
            return;

        CurrentStateType = Base_Attackstate;
    }
    private void StopCommand(InputAction.CallbackContext context)
    {
        CurrentStateType = _base_IDleState;
    }

    public override void UpdateMove()
    {
        Vector3 dir = new Vector3(_destPos.x, 0, _destPos.z) - new Vector3(transform.position.x, 0, transform.position.z);//높이에 대한 값을 빼야 근사값에 더 정확한 수치를 뽑을 수 있음.
        if (dir.magnitude < 0.01f)
        {
            CurrentStateType = _base_IDleState;
        }
        else
        {
            Debug.DrawRay(transform.position + Vector3.up * 0.5f, dir, Color.green);
            if (Physics.Raycast(transform.position + Vector3.up * 0.5f, dir, 1.0f, LayerMask.GetMask("Block")))
            {
                CurrentStateType = _base_IDleState;
                return;
            }
            float moveTick = Mathf.Clamp(_stats.MoveSpeed * Time.deltaTime, 0, dir.magnitude);
            _agent.Move(dir.normalized * moveTick);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 10 * Time.deltaTime);
        }
    }

    public void UpdatePickup()
    {
        ChangeAnimIfCurrentIsDone(_hash_PickUp, _base_IDleState);
    }
    public override void UpdateDie()
    {
    }
    public override void UpdateIdle()
    {

    }
    public override void UpdateAttack()
    {
        ChangeAnimIfCurrentIsDone(Hash_Attack, _base_IDleState);
    }
    protected override void AddInitalizeStateDict()
    {
        StateAnimDict.RegisterState(_pickup_State, () => RunAnimation(_hash_PickUp, DEFALUT_TRANSITION_PICKUP));
    }



    #region AnimationClipMethod
    public void AttackEvent()
    {
        TargetInSight.AttackTargetInSector(_stats);
    }

    #endregion
}
