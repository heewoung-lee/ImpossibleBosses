using System;
using System.Threading.Tasks;
using GameManagers.Interface.UIManager;
using Scene.CommonInstaller;
using Scene.GamePlayScene;
using UI.Scene.SceneUI;
using Unity.VisualScripting;
using UnityEngine;
using Util;
using Zenject;

namespace Scene.RoomScene
{
    public class RoomPlayScene : BaseScene,ISceneMultiMode,ISceneTestMode
    {
        [Inject]private IUISceneManager _uiSceneManager;
        [Inject]private ISceneConnectOnline _sceneConnectOnline;
        [Inject]private ISceneStarter _roomSceneStarter;

        
        [SerializeField] private TestMode testMode;
        [SerializeField] private MultiMode multiMode;
        public override Define.Scene CurrentScene => Define.Scene.RoomScene;
        public override ISceneSpawnBehaviour SceneSpawnBehaviour { get; }

        public TestMode GetTestMode()
        {
            return testMode;
        }

        public MultiMode GetMultiTestMode()
        {
            return multiMode;
        }


        public override void Clear()
        {
        }

        protected override void AwakeInit()
        {
        }
        protected override void StartInit()
        {
            base.StartInit();
            _ = StartInitAsync();
        }
        
        private async Task StartInitAsync()
        {
            try
            {
                await _sceneConnectOnline.SceneConnectOnlineStart();
                _roomSceneStarter.SceneStart();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[RoomPlayScene] 초기화 중 예외: {e}");
            }
        }
    }
}
