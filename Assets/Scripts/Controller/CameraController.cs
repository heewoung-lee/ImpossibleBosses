using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{

    private Define.CameraMode _currentCameraMode = Define.CameraMode.QuarterView;
    private Vector3 _quarterViewPosition = Define.DEFAULT_QUARTERVIEW_POSITION;
    private GameObject _player;
    private CursorController _cursorController;
    private InputAction _scrollAction;

    void Start()
    {
        _cursorController = gameObject.GetOrAddComponent<CursorController>();
        _scrollAction = Managers.InputManager.GetInputAction(Define.ControllerType.Camera, "Scroll");
        _scrollAction.performed += SetCameraFOV;
        _scrollAction.Enable();

    }
    public void SetCameraFOV(InputAction.CallbackContext context)
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        float scrollValue = context.ReadValue<float>();
        _quarterViewPosition = new Vector3(_quarterViewPosition.x, Mathf.Clamp(_quarterViewPosition.y + scrollValue, 3, 20), _quarterViewPosition.z);
    }
    void LateUpdate()
    {
        if (_currentCameraMode == Define.CameraMode.QuarterView)
        {
            RaycastHit hit;
            if (Physics.Raycast(_player.transform.position, _quarterViewPosition.normalized, out hit, _quarterViewPosition.magnitude, LayerMask.GetMask("Block")))
            {
                float dist = (hit.point - _player.transform.position).magnitude;//닿은 지점과 플레이어간의 거리 계산
                transform.position = Vector3.Slerp(transform.position,_player.transform.position + _quarterViewPosition.normalized * dist + Vector3.up,0.3f);
            }
            else
            {
                transform.position = _quarterViewPosition + _player.transform.position;
                transform.rotation = Define.DEFAULT_QUARTERVIEW_ROTATION;
                transform.LookAt(_player.transform);
            }
        }
    }
    public void SetQuarterViewMode(Vector3 setPosition)
    {
        _currentCameraMode = Define.CameraMode.QuarterView;
        _quarterViewPosition = setPosition;
    }


    public void SetPlayer(GameObject player)
    {
        _player = player;
    }

}
