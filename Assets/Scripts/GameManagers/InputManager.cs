using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameManagers
{
    public class InputManager : IManagerInitializable
    {
        private InputActionAsset _inputActionAsset;
        public InputActionAsset InputActionAsset
        {                                   
            get
            {
                if(_inputActionAsset == null)
                {
                    _inputActionAsset = Managers.ResourceManager.Load<InputActionAsset>("InputData/GameInputActions");
                }
                return _inputActionAsset;
            }
        }

        private Dictionary<string, Dictionary<string, InputAction>> _inputActionMapDict = new Dictionary<string, Dictionary<string, InputAction>>();

        //public Action<Vector3> playerMouseClickPositionEvent;
        //6.11일 플레이어의포지션에 클릭포지션에 따라 수행되는 이벤트 제거, 클래스가 수행하지 않아도될 책임을 지게 되는 터라 삭제함,
        public void Init()
        {
            _inputActionAsset = Managers.ResourceManager.Load<InputActionAsset>("InputData/GameInputActions");
            _inputActionMapDict = InitActionMapDict(_inputActionAsset);
        }

        private Dictionary<string, Dictionary<string, InputAction>> InitActionMapDict(InputActionAsset inputAssets)
        {
            Dictionary<string, Dictionary<string, InputAction>> actionMapDict = new Dictionary<string, Dictionary<string, InputAction>>();

            foreach (InputActionMap actionMap in inputAssets.actionMaps)
            {
                Dictionary<string, InputAction> actionDict = new Dictionary<string, InputAction>();
                foreach (InputAction action in actionMap)
                {
                    actionDict[action.name] = action;
                }
                actionMapDict[actionMap.name] = actionDict;
            }
            return actionMapDict;
        }


        public InputAction GetInputAction(Define.ControllerType controllerType,string actionName)
        {
            //타입으로 제일 처음 딕셔너리 찾기
            string controllerTypeString = controllerType.ToString();

            if (_inputActionMapDict[controllerTypeString] == null)
            {
                Debug.Log($"Not Found ActionMap: {controllerType}");
                return null;
            }

            if (_inputActionMapDict[controllerTypeString][actionName] == null)
            {
                Debug.Log($"Not Found Action: {actionName}");
                return null;
            }

            return _inputActionMapDict[controllerTypeString][actionName];
        }

    }
}
