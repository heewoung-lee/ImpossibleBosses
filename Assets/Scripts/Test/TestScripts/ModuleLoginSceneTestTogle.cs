using GameManagers;
using GameManagers.Interface;
using GameManagers.Interface.UI_Interface;
using GameManagers.Interface.UIManager;
using Test.TestUI;
using UnityEngine;
using Zenject;

namespace Test.TestScripts
{
    public class ModuleLoginSceneTestTogle : MonoBehaviour
    {
        [Inject] private IUISceneManager _sceneUIManager;

        void Start()
        {
            LogInTestToggle testTogle = _sceneUIManager.GetSceneUIFromResource<LogInTestToggle>(path: "Prefabs/UI/TestUI/LogInTestToggle");
        }
    }
}
