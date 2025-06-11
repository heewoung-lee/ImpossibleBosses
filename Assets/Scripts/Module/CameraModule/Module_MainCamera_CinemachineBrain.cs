using GameManagers;
using UnityEngine;

public class Module_MainCamera_CinemachineBrain : MonoBehaviour
{
    GameObject _mainCamera;
    void Start()
    {
        _mainCamera = GameObject.Find("CinemachineBrainCamera") == true ? GameObject.Find("CinemachineBrainCamera") :
            Managers.ResourceManager.Instantiate("Prefabs/Camera/CinemachineBrainCamera");

        _mainCamera.GetOrAddComponent<Module_Call_ToFollwingCamera>();
    }
}
