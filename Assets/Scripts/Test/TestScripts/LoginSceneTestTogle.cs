using GameManagers;
using Test.TestUI;
using UnityEngine;

namespace Test.TestScripts
{
    public class LoginSceneTestTogle : MonoBehaviour
    {
        void Start()
        {
            LogInTestToggle testTogle = Managers.UIManager.GetSceneUIFromResource<LogInTestToggle>(path: "Prefabs/UI/TestUI/LogInTestToggle");
        }
    }
}
