using System;
using Controller;
using Controller.ControllerStats.BaseStates;
using Controller.PlayerState;
using Data;
using GameManagers;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerController : MoveableController
    {
        private const float DefaultTransitionPickup = 0.3f;
        private InputManager _inputmanager;
        private NavMeshAgent _agent;
        private PlayerStats _stats;

        private PlayerInput _playerInput;
        private InputAction _moveAction;
        private InputAction _pointerAction;
        private InputAction _attackAction;
        private InputAction _stopAction;

        private Action<Vector3> _onPlayerMouseClickPosition;

        public event Action<Vector3> OnPlayerMouseClickPosition
        {
            add { UniqueEventRegister.AddSingleEvent(ref _onPlayerMouseClickPosition, value); }
            remove { UniqueEventRegister.RemovedEvent(ref _onPlayerMouseClickPosition, value); }
        }

        public Func<InputAction.CallbackContext, Vector3> ClickPositionEvent;
        public override Define.WorldObject WorldobjectType { get; protected set; } = Define.WorldObject.Player;
        protected override int HashIdle => PlayerAnimHash.Idle;
        protected override int HashMove => PlayerAnimHash.Run;
        protected override int HashAttack => PlayerAnimHash.Attack;
        protected override int HashDie => PlayerAnimHash.Die;
        private int _hashPickUp => Animator.StringToHash("Pickup");

        public override AttackState BaseAttackState => _baseAttackState;
        public override IDleState BaseIDleState => _baseIDleState;
        public override DieState BaseDieState => _baseDieState;
        public override MoveState BaseMoveState => _baseMoveState;

        public PickUpState PickupState => _pickupState;

        private AttackState _baseAttackState;
        private IDleState _baseIDleState;
        private DieState _baseDieState;
        private MoveState _baseMoveState;
        private PickUpState _pickupState;

        protected override void AwakeInit()
        {
            _stats = gameObject.GetComponent<PlayerStats>();
            _agent = gameObject.GetComponent<NavMeshAgent>();
            _playerInput = gameObject.GetComponent<PlayerInput>();

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

            _baseAttackState = new AttackState(UpdateAttack);
            _baseMoveState = new MoveState(UpdateMove);
            _baseDieState = new DieState(UpdateDie);
            _baseIDleState = new IDleState(UpdateIdle);
            _pickupState = new PickUpState(UpdatePickup);
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
                if (CurrentStateType != _baseMoveState)
                    CurrentStateType = _baseMoveState;
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
            _onPlayerMouseClickPosition?.Invoke(clickPos);
        }

        public void PlayerDead()
        {
            CurrentStateType = BaseDieState;
        }

        private void Attack(InputAction.CallbackContext context)
        {
            if (CurrentStateType == BaseAttackState)
                return;

            CurrentStateType = BaseAttackState;
        }

        private void StopCommand(InputAction.CallbackContext context)
        {
            CurrentStateType = _baseIDleState;
        }

        public override void UpdateMove()
        {
            Vector3 dir = new Vector3(_destPos.x, 0, _destPos.z) -
                          new Vector3(transform.position.x, 0,
                              transform.position.z); //높이에 대한 값을 빼야 근사값에 더 정확한 수치를 뽑을 수 있음.
            if (dir.magnitude < 0.01f)
            {
                CurrentStateType = _baseIDleState;
            }
            else
            {
                Debug.DrawRay(transform.position + Vector3.up * 0.5f, dir, Color.green);
                if (Physics.Raycast(transform.position + Vector3.up * 0.5f, dir, 1.0f, LayerMask.GetMask("Block")))
                {
                    CurrentStateType = _baseIDleState;
                    return;
                }

                float moveTick = Mathf.Clamp(_stats.MoveSpeed * Time.deltaTime, 0, dir.magnitude);
                _agent.Move(dir.normalized * moveTick);
                transform.rotation =
                    Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 10 * Time.deltaTime);
            }
        }

        public void UpdatePickup()
        {
            ChangeAnimIfCurrentIsDone(_hashPickUp, _baseIDleState);
        }

        public override void UpdateDie()
        {
        }

        public override void UpdateIdle()
        {
        }

        public override void UpdateAttack()
        {
            ChangeAnimIfCurrentIsDone(HashAttack, _baseIDleState);
        }

        protected override void AddInitalizeStateDict()
        {
            StateAnimDict.RegisterState(_pickupState, () => RunAnimation(_hashPickUp, DefaultTransitionPickup));
        }


        #region AnimationClipMethod

        public void AttackEvent()
        {
            TargetInSight.AttackTargetInSector(_stats);
        }

        #endregion
    }
}