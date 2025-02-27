using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerFollowingCamera : MonoBehaviour
{
    private Transform _playerTr;
    [SerializeField]private CinemachineCamera _camera;
    private CinemachineOrbitalFollow _cinemachineOrbitalFollow;
    private InputAction _mouseMiddleButton;
    private InputAction _mouseDelta;
    private InputAction _mouseScroll;

    private bool _mouseMiddleButtonPressed = false;
    private void Awake()
    {
        _camera = GetComponent<CinemachineCamera>();
        _cinemachineOrbitalFollow = GetComponent<CinemachineOrbitalFollow>();
        Managers.SocketEventManager.PlayerSpawnInitalize += InitalizeFollowingCamera;
    }

    public void InitalizeFollowingCamera(GameObject player)
    {
        _playerTr = player.transform;
        _camera.Target.TrackingTarget = _playerTr;

        _mouseMiddleButton = Managers.InputManager.GetInputAction(Define.ControllerType.Camera, "MouseScrollButton");
        _mouseMiddleButton.Enable();
        _mouseMiddleButton.started += context => _mouseMiddleButtonPressed = true;
        _mouseMiddleButton.canceled += context => _mouseMiddleButtonPressed = false;


        _mouseDelta = Managers.InputManager.GetInputAction(Define.ControllerType.Camera, "Look");
        _mouseDelta.Enable();
        _mouseDelta.performed += PressedMiddleMouseButton;


        _mouseScroll = Managers.InputManager.GetInputAction(Define.ControllerType.Camera, "Scroll");
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