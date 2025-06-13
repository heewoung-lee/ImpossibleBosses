using Player;
using UnityEngine;

namespace Module.CameraModule
{
    public class ModulePlayerRenderTextureCamara : MonoBehaviour
    {
        private Transform _player; // 플레이어 Transform


        void Start()
        {
            _player = GetComponentInParent<PlayerController>().transform;
        }

        void LateUpdate()
        {
            // 카메라의 위치를 플레이어 기준으로 조정
            // (플레이어의 정면에서 약간 위쪽에서 바라보는 위치로 설정)
            Vector3 cameraOffset = new Vector3(0f, 1.3f, -3f); // 정면에서 약간 위
            transform.position = _player.position - _player.forward * cameraOffset.z + Vector3.up * cameraOffset.y;

            // 플레이어를 바라보도록 설정
            transform.LookAt(_player.position + Vector3.up * 1.3f); // 플레이어의 머리 높이로 조정

            // 필요하면 추가적인 회전 보정
            transform.rotation *= Quaternion.Euler(15f, 0f, 0f); // 위쪽에서 바라보는 느낌 추가
        }
    }
}
