using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class BaseScene : MonoBehaviour
{
    public abstract Define.Scene CurrentScene { get; }
    MoveMarkerController _moveMarker;

    void Start()
    {
        StartInit();    
    }

    private void Awake()
    {
        AwakeInit();
    }

    protected virtual void StartInit()
    {
        Object go = GameObject.FindAnyObjectByType<EventSystem>();
        if (go == null)
        {
           Managers.ResourceManager.Instantiate("Prefabs/UI/EventSystem").name = "@EventSystem";
        }
        _moveMarker = gameObject.GetOrAddComponent<MoveMarkerController>();
    }

    protected abstract void AwakeInit();

    public abstract void Clear();
}
