using GameManagers;
using GameManagers.Interface;
using Test.TestUI;
using UnityEngine;
using Zenject;

namespace Test.TestScripts
{
    public class LoginSceneTestTogle : MonoBehaviour
    {
        [Inject] private IUISceneManager _sceneUIManager;

        void Start()
        {
            LogInTestToggle testTogle = _sceneUIManager.GetSceneUIFromResource<LogInTestToggle>(path: "Prefabs/UI/TestUI/LogInTestToggle");
        }
    }
}
