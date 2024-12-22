using Unity.Cinemachine;
using UnityEngine;

public class PlayerFollowingCamera : MonoBehaviour
{
    private PlayerController _playerController;
    private CinemachineCamera _camera;

    private void Awake()
    {
        _camera = GetComponent<CinemachineCamera>();
    }
    private void Start()
    {
    }
}