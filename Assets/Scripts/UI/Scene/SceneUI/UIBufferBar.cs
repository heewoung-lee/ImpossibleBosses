using Buffer;
using GameManagers;
using GameManagers.Interface.BufferManager;
using UI.Scene.Interface;
using UnityEngine;
using Zenject;

namespace UI.Scene.SceneUI
{
    public class UIBufferBar : UIScene,ISceneChangeBehaviour
    {
    
        [Inject]private IBufferManager _bufferManager;
        [Inject] SceneManagerEx _sceneManagerEx;
        enum BufferContextTr
        {
            BufferContext
        }


        private Transform _bufferContext;

        public Transform BufferContext { get => _bufferContext; }//여기에 다른애들이 추가를 한다면, 


        protected override void AwakeInit()
        {
            base.AwakeInit();
        }


        protected override void OnEnableInit()
        {
            base.OnEnableInit();
            _sceneManagerEx.OnBeforeSceneUnloadLocalEvent += OnBeforeSceneUnload;
        }

        protected override void OnDisableInit()
        {
            base.OnDisableInit();
            _sceneManagerEx.OnBeforeSceneUnloadLocalEvent -= OnBeforeSceneUnload;

        }
        protected override void StartInit()
        {
            Bind<Transform>(typeof(BufferContextTr));
            _bufferContext = Get<Transform>((int)BufferContextTr.BufferContext);
        }

        public void OnBeforeSceneUnload()
        {
            foreach(BufferComponent buffer in _bufferContext.GetComponentsInChildren(typeof(BufferComponent)))
            {
                _bufferManager.RemoveBuffer(buffer);
            }
        }
    }
}
