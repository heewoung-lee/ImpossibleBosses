using GameManagers;
using Test.TestUI;
using UnityEngine;
using Zenject;

namespace Test.TestScripts
{
    public class LoginSceneTestTogle : MonoBehaviour
    {
        [Inject] private UIManager _uiManager;

        void Start()
        {
            LogInTestToggle testTogle = _uiManager.GetSceneUIFromResource<LogInTestToggle>(path: "Prefabs/UI/TestUI/LogInTestToggle");
        }
    }
}
