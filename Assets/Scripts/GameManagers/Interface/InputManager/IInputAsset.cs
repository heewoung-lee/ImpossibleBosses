using UnityEngine.InputSystem;
using Util;

namespace GameManagers.Interface.InputManager
{
    public interface IInputAsset
    {
        public InputActionAsset GetInputActionAsset();
        public InputAction GetInputAction(Define.ControllerType controllerType, string actionName);
    }
}