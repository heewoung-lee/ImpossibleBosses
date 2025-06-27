using Controller;
using GameManagers;
using GameManagers.Interface.Resources_Interface;
using GameManagers.Interface.ResourcesManager;
using UnityEngine;
using Util;
using Zenject;

namespace Module.CameraModule
{
    public class ModuleCallToFollwingCamera : MonoBehaviour
    {
        private GameObject _playerFollwingCamera;
        [Inject] private IInstantiate _instantiate;
        void Start()
        {
            _playerFollwingCamera = GameObject.Find("PlayerFollowingCamera") == true ? 
                GameObject.Find("PlayerFollowingCamera") : _instantiate.InstantiateByPath("Prefabs/Camera/PlayerFollowingCamera");
            _instantiate.GetOrAddComponent<PlayerFollowingCamera>(_playerFollwingCamera);
        }

    }
}
