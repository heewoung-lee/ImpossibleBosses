using System;
using Controller;
using GameManagers;
using GameManagers.Interface.Resources_Interface;
using Scene.GamePlayScene;
using UnityEngine;
using UnityEngine.EventSystems;
using Util;
using Zenject;
using Object = UnityEngine.Object;

namespace Scene
{
    public abstract class BaseScene : MonoBehaviour
    {
        [Inject] private IInstantiate _instantiate;
        public abstract Define.Scene CurrentScene { get; }
        public abstract ISceneSpawnBehaviour SceneSpawnBehaviour { get; }
        MoveMarkerController _moveMarker;

        void Start()
        {
            StartInit();
        }

        private void Awake()
        {
#if UNITY_EDITOR
            if (FindAnyObjectByType<SceneContext>() == null)
            { 
                Debug.LogWarning("There is no Scene Context");
            }
#endif
            AwakeInit();
        }

        protected virtual void StartInit()
        {
            Object go = GameObject.FindAnyObjectByType<EventSystem>();
            if (go == null)
            {
                _instantiate.InstantiateByPath("Prefabs/UI/EventSystem").name = "@EventSystem";
            }
            _moveMarker = _instantiate.GetOrAddComponent<MoveMarkerController>(gameObject);
        }

        protected abstract void AwakeInit();

        public abstract void Clear();

    }
}