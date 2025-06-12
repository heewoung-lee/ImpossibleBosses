using GameManagers;
using UnityEngine;

namespace Module.CameraModule
{
    public class ModuleMainCameraCinemachineBrain : MonoBehaviour
    {
        GameObject _mainCamera;
        void Start()
        {
            _mainCamera = GameObject.Find("CinemachineBrainCamera") == true ? GameObject.Find("CinemachineBrainCamera") :
                Managers.ResourceManager.Instantiate("Prefabs/Camera/CinemachineBrainCamera");

            _mainCamera.GetOrAddComponent<ModuleCallToFollwingCamera>();
        }
    }
}
