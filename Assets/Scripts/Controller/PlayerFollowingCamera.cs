using GameManagers;
using GameManagers.Interface.GameManagerEx;
using GameManagers.Interface.InputManager;
using Stats;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Util;
using Zenject;

namespace Controller
{
    public class PlayerFollowingCamera : MonoBehaviour
    {
        private Transform _playerTr;
        private CinemachineCamera _camera;
        private CinemachineOrbitalFollow _cinemachineOrbitalFollow;
        private InputAction _mouseMiddleButton;
        private InputAction _mouseDelta;
        private InputAction _mouseScroll;
        [Inject] private IInputAsset _inputManager;
        [Inject] IPlayerSpawnManager _gameManagerEx;

        private bool _mouseMiddleButtonPressed = false;
        private void Awake()
        {
            _camera = GetComponent<CinemachineCamera>();
            _cinemachineOrbitalFollow = GetComponent<CinemachineOrbitalFollow>();

            if(_gameManagerEx.GetPlayer() == null)
            {
                _gameManagerEx.OnPlayerSpawnEvent += InitializeFollowingCamera;
            }
            else
            {
                InitializeFollowingCamera(_gameManagerEx.GetPlayer().GetComponent<PlayerStats>());
            }
        }

        public void InitializeFollowingCamera(PlayerStats playerstats)
        {
            _playerTr = playerstats.transform;
            _camera.Target.TrackingTarget = _playerTr;

            _mouseMiddleButton = _inputManager.GetInputAction(Define.ControllerType.Camera, "MouseScrollButton");
            _mouseMiddleButton.Enable();
            _mouseMiddleButton.started += context => _mouseMiddleButtonPressed = true;
            _mouseMiddleButton.canceled += context => _mouseMiddleButtonPressed = false;


            _mouseDelta = _inputManager.GetInputAction(Define.ControllerType.Camera, "Look");
            _mouseDelta.Enable();
            _mouseDelta.performed += PressedMiddleMouseButton;


            _mouseScroll = _inputManager.GetInputAction(Define.ControllerType.Camera, "Scroll");
            _mouseScroll.Enable();
            _mouseScroll.performed += SetCameraHeight;
        }

        public void PressedMiddleMouseButton(InputAction.CallbackContext context)
        {
            if (_mouseMiddleButtonPressed)
            {
                _cinemachineOrbitalFollow.HorizontalAxis.Value += context.ReadValue<Vector2>().x *0.1f;
            }
        }

        public void SetCameraHeight(InputAction.CallbackContext context)
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;
            _cinemachineOrbitalFollow.VerticalAxis.Value += context.ReadValue<Vector2>().y;
            _cinemachineOrbitalFollow.VerticalAxis.Value = Mathf.Clamp(_cinemachineOrbitalFollow.VerticalAxis.Value, _cinemachineOrbitalFollow.VerticalAxis.Range.x, _cinemachineOrbitalFollow.VerticalAxis.Range.y);
        }

    }
}