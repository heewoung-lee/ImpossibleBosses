using Controller;
using GameManagers;
using UnityEngine;

namespace Module.CameraModule
{
    public class ModuleCallToFollwingCamera : MonoBehaviour
    {
        private GameObject _playerFollwingCamera;
        void Start()
        {
            _playerFollwingCamera = GameObject.Find("PlayerFollowingCamera") == true ? 
                GameObject.Find("PlayerFollowingCamera") : Managers.ResourceManager.Instantiate("Prefabs/Camera/PlayerFollowingCamera");
            _playerFollwingCamera.GetOrAddComponent<PlayerFollowingCamera>();
        }

    }
}
