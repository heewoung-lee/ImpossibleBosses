using GameManagers;
using GameManagers.Interface.Resources_Interface;
using UnityEngine;
using Util;
using Zenject;

namespace Module.CameraModule
{
    public class ModuleMainCameraCinemachineBrain : MonoBehaviour
    {
        [Inject] private IInstantiate _instantiate;
        GameObject _mainCamera;
        void Start()
        {
            _mainCamera = GameObject.Find("CinemachineBrainCamera") == true ? GameObject.Find("CinemachineBrainCamera") :
                _instantiate.InstantiateByPath("Prefabs/Camera/CinemachineBrainCamera");

            _instantiate.GetOrAddComponent<ModuleCallToFollwingCamera>(_mainCamera);
        }
    }
}
