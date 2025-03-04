using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

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

    public Action<Vector3> playerMouseClickPositionEvent;
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
