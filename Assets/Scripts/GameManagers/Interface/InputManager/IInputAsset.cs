using System.Collections.Generic;
using UnityEngine.InputSystem;
using Util;

namespace GameManagers.Interface.InputManager_Interface
{
    public interface IInputAsset
    {
        public InputActionAsset InputActionAsset { get; }
        public InputAction GetInputAction(Define.ControllerType controllerType, string actionName);
        public Dictionary<string, Dictionary<string, InputAction>> InputActionMapDict { get; }

    }
}