using System;
using System.Threading.Tasks;
using Controller;
using GameManagers;
using GameManagers.Interface.Resources_Interface;
using GameManagers.Interface.ResourcesManager;
using GameManagers.Interface.UIManager;
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
        [Inject] private IUISceneManager _uiSceneManager;
        [Inject] private LobbyManager _lobbyManager;
        [Inject] private SceneManagerEx _sceneManagerEx;
        public abstract Define.Scene CurrentScene { get; }
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
        }

        protected virtual void AwakeInit()  {}
        public virtual void Clear() {}
    }
}